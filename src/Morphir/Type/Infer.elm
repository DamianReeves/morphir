module Morphir.Type.Infer exposing (..)

import Dict exposing (Dict)
import Morphir.IR.FQName exposing (FQName, fqn)
import Morphir.IR.Literal exposing (Literal(..))
import Morphir.IR.Name exposing (Name)
import Morphir.IR.Type as Type exposing (Type)
import Morphir.IR.Value as Value exposing (Pattern(..), Value)
import Morphir.ListOfResults as ListOfResults
import Morphir.Type.Class as Class exposing (Class)
import Morphir.Type.Constraint as Constraint exposing (Constraint(..), class, equality)
import Morphir.Type.ConstraintSet as ConstraintSet exposing (ConstraintSet(..))
import Morphir.Type.MetaType as MetaType exposing (MetaType(..))
import Morphir.Type.MetaVar as MetaVar exposing (Variable)
import Morphir.Type.SolutionMap as SolutionMap exposing (SolutionMap(..))


type alias TypedValue va =
    Value () ( va, Type () )


type TypeError
    = UnsolvableConstraints (List Constraint)
    | TypeMismatch MetaType MetaType
    | TypeErrors (List TypeError)
    | ClassConstraintViolation MetaType Class


infer : Value () va -> Result TypeError (TypedValue va)
infer untypedValue =
    let
        annotatedValue : Value () ( va, Variable )
        annotatedValue =
            annotate untypedValue

        constraints : ConstraintSet
        constraints =
            constrainValue Dict.empty annotatedValue

        solution : Result TypeError ( ConstraintSet, SolutionMap )
        solution =
            solve constraints
    in
    solution
        |> Result.map (applySolution annotatedValue)


type alias AnnotatedValue va =
    Value () ( va, Variable )


annotate : Value () va -> AnnotatedValue va
annotate untypedValue =
    untypedValue
        |> Value.indexedMapValue (\index va -> ( va, MetaVar.variable index )) 0
        |> Tuple.first


constrainValue : Dict Name Variable -> AnnotatedValue va -> ConstraintSet
constrainValue vars annotatedValue =
    case annotatedValue of
        Value.Literal ( _, thisTypeVar ) literalValue ->
            constrainLiteral thisTypeVar literalValue

        --Value.Constructor ( _, thisTypeVar ) fQName ->
        Value.Tuple ( _, thisTypeVar ) elems ->
            let
                elemsConstraints : List ConstraintSet
                elemsConstraints =
                    elems
                        |> List.map (constrainValue vars)

                tupleConstraint : ConstraintSet
                tupleConstraint =
                    ConstraintSet.singleton
                        (equality
                            (MetaVar thisTypeVar)
                            (elems
                                |> List.map metaTypeVarForValue
                                |> MetaTuple
                            )
                        )
            in
            ConstraintSet.concat (tupleConstraint :: elemsConstraints)

        Value.List ( _, thisTypeVar ) items ->
            let
                itemType : MetaType
                itemType =
                    MetaVar (thisTypeVar |> MetaVar.subVariable)

                listConstraint : Constraint
                listConstraint =
                    equality (MetaVar thisTypeVar) (MetaType.listType itemType)

                itemConstraints : ConstraintSet
                itemConstraints =
                    items
                        |> List.map
                            (\item ->
                                constrainValue vars item
                                    |> ConstraintSet.insert (equality (metaTypeVarForValue item) itemType)
                            )
                        |> ConstraintSet.concat
            in
            itemConstraints
                |> ConstraintSet.insert listConstraint

        Value.Record ( _, thisTypeVar ) fieldValues ->
            let
                fieldConstraints : ConstraintSet
                fieldConstraints =
                    fieldValues
                        |> List.map (Tuple.second >> constrainValue vars)
                        |> ConstraintSet.concat

                recordType : MetaType
                recordType =
                    fieldValues
                        |> List.map
                            (\( fieldName, fieldValue ) ->
                                ( fieldName, metaTypeVarForValue fieldValue )
                            )
                        |> Dict.fromList
                        |> MetaRecord Nothing

                recordConstraints : ConstraintSet
                recordConstraints =
                    ConstraintSet.singleton
                        (equality (MetaVar thisTypeVar) recordType)
            in
            ConstraintSet.concat
                [ fieldConstraints
                , recordConstraints
                ]

        Value.Variable ( _, varUse ) varName ->
            case vars |> Dict.get varName of
                Just varDecl ->
                    ConstraintSet.singleton (equality (MetaVar varUse) (MetaVar varDecl))

                Nothing ->
                    -- this should never happen if variables were validated earlier
                    ConstraintSet.empty

        --Value.Field ( _, thisTypeVar ) subjectValue fieldName ->
        --
        Value.Apply ( _, thisTypeVar ) funValue argValue ->
            let
                funType : MetaType
                funType =
                    MetaFun
                        (metaTypeVarForValue argValue)
                        (MetaVar thisTypeVar)

                applyConstraints : ConstraintSet
                applyConstraints =
                    ConstraintSet.singleton
                        (equality (metaTypeVarForValue funValue) funType)
            in
            ConstraintSet.concat
                [ constrainValue vars funValue
                , constrainValue vars argValue
                , applyConstraints
                ]

        Value.Lambda ( _, thisTypeVar ) argPattern bodyValue ->
            let
                ( argVariables, argConstraints ) =
                    constrainPattern argPattern

                lambdaType : MetaType
                lambdaType =
                    MetaFun
                        (metaTypeVarForPattern argPattern)
                        (metaTypeVarForValue bodyValue)

                lambdaConstraints : ConstraintSet
                lambdaConstraints =
                    ConstraintSet.singleton
                        (equality (MetaVar thisTypeVar) lambdaType)
            in
            ConstraintSet.concat
                [ lambdaConstraints
                , constrainValue (Dict.union argVariables vars) bodyValue
                , argConstraints
                ]

        Value.Destructure ( _, thisTypeVar ) bindPattern bindValue inValue ->
            let
                ( bindPatternVariables, bindPatternConstraints ) =
                    constrainPattern bindPattern

                bindValueConstraints : ConstraintSet
                bindValueConstraints =
                    constrainValue vars bindValue

                inValueConstraints : ConstraintSet
                inValueConstraints =
                    constrainValue (Dict.union bindPatternVariables vars) inValue

                destructureConstraints : ConstraintSet
                destructureConstraints =
                    ConstraintSet.fromList
                        [ equality (MetaVar thisTypeVar) (metaTypeVarForValue inValue)
                        , equality (metaTypeVarForValue bindValue) (metaTypeVarForPattern bindPattern)
                        ]
            in
            ConstraintSet.concat
                [ bindPatternConstraints
                , bindValueConstraints
                , inValueConstraints
                , destructureConstraints
                ]

        Value.IfThenElse ( _, thisTypeVar ) condition thenBranch elseBranch ->
            let
                specificConstraints : ConstraintSet
                specificConstraints =
                    ConstraintSet.fromList
                        -- the condition should always be bool
                        [ equality (metaTypeVarForValue condition) MetaType.boolType

                        -- the two branches should have the same type
                        , equality (metaTypeVarForValue elseBranch) (metaTypeVarForValue thenBranch)

                        -- the final type should be the same as the branches (can use any branch thanks to previous rule)
                        , equality (MetaVar thisTypeVar) (metaTypeVarForValue thenBranch)
                        ]

                childConstraints : List ConstraintSet
                childConstraints =
                    [ constrainValue vars condition
                    , constrainValue vars thenBranch
                    , constrainValue vars elseBranch
                    ]
            in
            ConstraintSet.concat (specificConstraints :: childConstraints)

        Value.PatternMatch ( _, thisTypeVar ) subjectValue cases ->
            let
                thisType : MetaType
                thisType =
                    MetaVar thisTypeVar

                subjectType : MetaType
                subjectType =
                    metaTypeVarForValue subjectValue

                casesConstraints : List ConstraintSet
                casesConstraints =
                    cases
                        |> List.map
                            (\( casePattern, caseValue ) ->
                                let
                                    ( casePatternVariables, casePatternConstraints ) =
                                        constrainPattern casePattern

                                    caseValueConstraints : ConstraintSet
                                    caseValueConstraints =
                                        constrainValue (Dict.union casePatternVariables vars) caseValue

                                    caseConstraints : ConstraintSet
                                    caseConstraints =
                                        ConstraintSet.fromList
                                            [ equality subjectType (metaTypeVarForPattern casePattern)
                                            , equality thisType (metaTypeVarForValue caseValue)
                                            ]
                                in
                                ConstraintSet.concat
                                    [ casePatternConstraints
                                    , caseValueConstraints
                                    , caseConstraints
                                    ]
                            )
            in
            ConstraintSet.concat casesConstraints

        Value.Unit ( _, thisTypeVar ) ->
            ConstraintSet.singleton
                (equality (MetaVar thisTypeVar) MetaUnit)

        other ->
            Debug.todo ("implement " ++ Debug.toString other)


constrainPattern : Pattern ( va, Variable ) -> ( Dict Name Variable, ConstraintSet )
constrainPattern untypedPattern =
    case untypedPattern of
        Value.WildcardPattern _ ->
            ( Dict.empty, ConstraintSet.empty )

        Value.AsPattern ( _, thisTypeVar ) nestedPattern alias ->
            let
                ( nestedVariables, nestedConstraints ) =
                    constrainPattern nestedPattern

                thisPatternConstraints : ConstraintSet
                thisPatternConstraints =
                    ConstraintSet.singleton
                        (equality (MetaVar thisTypeVar) (metaTypeVarForPattern nestedPattern))
            in
            ( nestedVariables |> Dict.insert alias thisTypeVar
            , ConstraintSet.union nestedConstraints thisPatternConstraints
            )

        Value.TuplePattern ( _, thisTypeVar ) elemPatterns ->
            let
                ( elemsVariables, elemsConstraints ) =
                    elemPatterns
                        |> List.map constrainPattern
                        |> List.unzip

                tupleConstraint : ConstraintSet
                tupleConstraint =
                    ConstraintSet.singleton
                        (equality
                            (MetaVar thisTypeVar)
                            (elemPatterns
                                |> List.map metaTypeVarForPattern
                                |> MetaTuple
                            )
                        )
            in
            ( List.foldl Dict.union Dict.empty elemsVariables
            , ConstraintSet.concat (tupleConstraint :: elemsConstraints)
            )

        --Value.ConstructorPattern ( _, thisTypeVar ) fQName argPatterns ->
        --    let
        --        ctorType : MetaType
        --        ctorType =
        --            MetaVar (thisTypeVar |> MetaVar.subVariable)
        --
        --
        --    in
        Value.EmptyListPattern ( _, thisTypeVar ) ->
            let
                itemType : MetaType
                itemType =
                    MetaVar (thisTypeVar |> MetaVar.subVariable)

                listType : MetaType
                listType =
                    MetaType.listType itemType
            in
            ( Dict.empty
            , ConstraintSet.singleton
                (equality (MetaVar thisTypeVar) listType)
            )

        Value.HeadTailPattern ( _, thisTypeVar ) headPattern tailPattern ->
            let
                ( headVariables, headConstraints ) =
                    constrainPattern headPattern

                ( tailVariables, tailConstraints ) =
                    constrainPattern tailPattern

                itemType : MetaType
                itemType =
                    metaTypeVarForPattern headPattern

                listType : MetaType
                listType =
                    MetaType.listType itemType

                thisPatternConstraints : ConstraintSet
                thisPatternConstraints =
                    ConstraintSet.fromList
                        [ equality (MetaVar thisTypeVar) listType
                        , equality (metaTypeVarForPattern tailPattern) listType
                        ]
            in
            ( Dict.union headVariables tailVariables
            , ConstraintSet.concat
                [ headConstraints, tailConstraints, thisPatternConstraints ]
            )

        Value.LiteralPattern ( _, thisTypeVar ) literalValue ->
            ( Dict.empty, constrainLiteral thisTypeVar literalValue )

        Value.UnitPattern ( _, thisTypeVar ) ->
            ( Dict.empty
            , ConstraintSet.singleton
                (equality (MetaVar thisTypeVar) MetaUnit)
            )

        other ->
            Debug.todo ("implement " ++ Debug.toString other)


constrainLiteral : Variable -> Literal -> ConstraintSet
constrainLiteral thisTypeVar literalValue =
    let
        expectExactType : MetaType -> ConstraintSet
        expectExactType expectedType =
            ConstraintSet.singleton
                (equality
                    (MetaVar thisTypeVar)
                    expectedType
                )
    in
    case literalValue of
        BoolLiteral _ ->
            expectExactType MetaType.boolType

        CharLiteral _ ->
            expectExactType MetaType.charType

        StringLiteral _ ->
            expectExactType MetaType.stringType

        IntLiteral _ ->
            ConstraintSet.singleton
                (class (MetaVar thisTypeVar) Class.Number)

        FloatLiteral _ ->
            expectExactType MetaType.floatType


solve : ConstraintSet -> Result TypeError ( ConstraintSet, SolutionMap )
solve constraintSet =
    solveHelp SolutionMap.empty constraintSet


solveHelp : SolutionMap -> ConstraintSet -> Result TypeError ( ConstraintSet, SolutionMap )
solveHelp solutionsSoFar ((ConstraintSet constraints) as constraintSet) =
    constraints
        |> validateConstraints
        |> Result.map removeTrivialConstraints
        |> Result.andThen
            (\nonTrivialConstraints ->
                nonTrivialConstraints
                    |> findSubstitution
                    |> Result.andThen
                        (\maybeNewSolutions ->
                            case maybeNewSolutions of
                                Nothing ->
                                    Ok ( ConstraintSet.fromList nonTrivialConstraints, solutionsSoFar )

                                Just newSolutions ->
                                    solutionsSoFar
                                        |> mergeSolutions newSolutions
                                        |> Result.andThen
                                            (\mergedSolutions ->
                                                solveHelp mergedSolutions (constraintSet |> ConstraintSet.applySubstitutions mergedSolutions)
                                            )
                        )
            )


removeTrivialConstraints : List Constraint -> List Constraint
removeTrivialConstraints constraints =
    constraints
        |> List.filter
            (\constraint ->
                case constraint of
                    Equality metaType1 metaType2 ->
                        metaType1 /= metaType2

                    Class metaType _ ->
                        case metaType of
                            -- If this is a variable we still need to resolve it
                            MetaVar _ ->
                                True

                            -- Otherwise it's a specific type already so we can remove this constraint
                            _ ->
                                False
            )


validateConstraints : List Constraint -> Result TypeError (List Constraint)
validateConstraints constraints =
    constraints
        |> List.map
            (\constraint ->
                case constraint of
                    Class (MetaVar _) _ ->
                        Ok constraint

                    Class metaType class ->
                        if Class.member metaType class then
                            Ok constraint

                        else
                            Err (ClassConstraintViolation metaType class)

                    _ ->
                        Ok constraint
            )
        |> ListOfResults.liftAllErrors
        |> Result.mapError typeErrors


findSubstitution : List Constraint -> Result TypeError (Maybe SolutionMap)
findSubstitution constraints =
    case constraints of
        [] ->
            Ok Nothing

        firstConstraint :: restOfConstraints ->
            case firstConstraint of
                Equality metaType1 metaType2 ->
                    unifyMetaType metaType1 metaType2
                        |> Result.andThen
                            (\solutions ->
                                if SolutionMap.isEmpty solutions then
                                    findSubstitution restOfConstraints

                                else
                                    Ok (Just solutions)
                            )

                Class _ _ ->
                    findSubstitution restOfConstraints


addSolution : Variable -> MetaType -> SolutionMap -> Result TypeError SolutionMap
addSolution var newSolution (SolutionMap currentSolutions) =
    case Dict.get var currentSolutions of
        Just existingSolution ->
            -- Unify with the existing solution
            unifyMetaType existingSolution newSolution
                |> Result.map
                    (\(SolutionMap newSubstitutions) ->
                        -- If it unifies apply the substitutions to the existing solution and add all new substitutions
                        SolutionMap
                            (currentSolutions
                                |> Dict.insert var
                                    (existingSolution
                                        |> MetaType.substituteVariables (Dict.toList newSubstitutions)
                                    )
                                |> Dict.union newSubstitutions
                            )
                    )

        Nothing ->
            -- Simply substitute and insert the new solution
            currentSolutions
                |> Dict.insert var newSolution
                |> SolutionMap
                |> SolutionMap.substituteVariable var newSolution
                |> Ok


mergeSolutions : SolutionMap -> SolutionMap -> Result TypeError SolutionMap
mergeSolutions (SolutionMap newSolutions) currentSolutions =
    newSolutions
        |> Dict.toList
        |> List.foldl
            (\( var, newSolution ) solutionsSoFar ->
                solutionsSoFar
                    |> Result.andThen (addSolution var newSolution)
            )
            (Ok currentSolutions)


unifyMetaType : MetaType -> MetaType -> Result TypeError SolutionMap
unifyMetaType metaType1 metaType2 =
    if metaType1 == metaType2 then
        Ok SolutionMap.empty

    else
        case ( metaType1, metaType2 ) of
            ( MetaVar var1, _ ) ->
                unifyVariable var1 metaType2

            ( _, MetaVar var2 ) ->
                unifyVariable var2 metaType1

            ( MetaTuple elems1, _ ) ->
                unifyTuple elems1 metaType2

            ( MetaRef ref1, _ ) ->
                unifyRef ref1 metaType2

            ( MetaApply fun1 arg1, _ ) ->
                unifyApply fun1 arg1 metaType2

            other ->
                Debug.todo ("implement " ++ Debug.toString other)


unifyVariable : Variable -> MetaType -> Result TypeError SolutionMap
unifyVariable var1 metaType2 =
    Ok (SolutionMap.singleton var1 metaType2)


unifyTuple : List MetaType -> MetaType -> Result TypeError SolutionMap
unifyTuple elems1 metaType2 =
    case metaType2 of
        MetaTuple elems2 ->
            if List.length elems1 == List.length elems2 then
                List.map2 unifyMetaType elems1 elems2
                    |> ListOfResults.liftAllErrors
                    |> Result.mapError TypeErrors
                    |> Result.andThen
                        (\solutionMaps ->
                            solutionMaps
                                |> List.foldl
                                    (\nextSolutions resultSoFar ->
                                        resultSoFar
                                            |> Result.andThen
                                                (\solutionsSoFar ->
                                                    mergeSolutions solutionsSoFar nextSolutions
                                                )
                                    )
                                    (Ok SolutionMap.empty)
                        )

            else
                Debug.todo "implement"

        _ ->
            Debug.todo "implement"


unifyRef : FQName -> MetaType -> Result TypeError SolutionMap
unifyRef ref1 metaType2 =
    case metaType2 of
        MetaRef ref2 ->
            if ref1 == ref2 then
                Ok SolutionMap.empty

            else
                Err (TypeMismatch (MetaRef ref1) metaType2)

        _ ->
            Debug.todo "implement"


unifyApply : MetaType -> MetaType -> MetaType -> Result TypeError SolutionMap
unifyApply fun1 arg1 metaType2 =
    case metaType2 of
        MetaApply fun2 arg2 ->
            Result.andThen identity
                (Result.map2 mergeSolutions
                    (unifyMetaType fun1 fun2)
                    (unifyMetaType arg1 arg2)
                )

        _ ->
            Debug.todo "implement"


applySolution : AnnotatedValue va -> ( ConstraintSet, SolutionMap ) -> TypedValue va
applySolution annotatedValue ( residualConstraints, solutionMap ) =
    annotatedValue
        |> Value.mapValueAttributes identity
            (\( va, metaVar ) ->
                ( va
                , solutionMap
                    |> SolutionMap.get metaVar
                    |> Maybe.map (metaToConcreteType solutionMap)
                    |> Maybe.withDefault (metaVar |> MetaVar.toName |> Type.Variable ())
                )
            )


metaToConcreteType : SolutionMap -> MetaType -> Type ()
metaToConcreteType solutionMap metaType =
    case metaType of
        MetaVar metaVar ->
            solutionMap
                |> SolutionMap.get metaVar
                |> Maybe.map (metaToConcreteType solutionMap)
                |> Maybe.withDefault (metaVar |> MetaVar.toName |> Type.Variable ())

        MetaTuple metaElems ->
            Type.Tuple ()
                (metaElems
                    |> List.map (metaToConcreteType solutionMap)
                )

        MetaRecord extends metaFields ->
            case extends of
                Nothing ->
                    Type.Record ()
                        (metaFields
                            |> Dict.toList
                            |> List.map
                                (\( fieldName, fieldType ) ->
                                    Type.Field fieldName
                                        (metaToConcreteType solutionMap fieldType)
                                )
                        )

                Just baseType ->
                    Debug.todo "implement extensible record"

        --Type.ExtensibleRecord ()
        --    (metaFields
        --        |> Dict.toList
        --        |> List.map
        --            (\( fieldName, fieldType ) ->
        --                Type.Field fieldName
        --                    (metaToConcreteType solutionMap fieldType)
        --            )
        --    )
        MetaApply _ _ ->
            let
                uncurry mt =
                    case mt of
                        MetaApply mf ma ->
                            let
                                ( f, args ) =
                                    uncurry mf
                            in
                            ( f, args ++ [ ma ] )

                        _ ->
                            ( mt, [] )

                ( metaFun, metaArgs ) =
                    uncurry metaType
            in
            case metaFun of
                MetaRef fQName ->
                    metaArgs
                        |> List.map (metaToConcreteType solutionMap)
                        |> Type.Reference () fQName

                other ->
                    metaToConcreteType solutionMap other

        MetaFun argType returnType ->
            Type.Function ()
                (metaToConcreteType solutionMap argType)
                (metaToConcreteType solutionMap returnType)

        MetaRef fQName ->
            Type.Reference () fQName []

        MetaUnit ->
            Type.Unit ()


typeErrors : List TypeError -> TypeError
typeErrors errors =
    case errors of
        [ single ] ->
            single

        _ ->
            TypeErrors errors


metaTypeVarForValue : AnnotatedValue va -> MetaType
metaTypeVarForValue value =
    value
        |> Value.valueAttribute
        |> Tuple.second
        |> MetaVar


metaTypeVarForPattern : Pattern ( va, Variable ) -> MetaType
metaTypeVarForPattern pattern =
    pattern
        |> Value.patternAttribute
        |> Tuple.second
        |> MetaVar
