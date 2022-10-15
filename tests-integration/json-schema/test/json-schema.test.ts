const  Ajv2020 = require("../../../node_modules/ajv/dist/2020")
const fs = require('fs')
const basePath = "tests-integration/json-schema/model/"
const schemaBasePath = "tests-integration/generated/jsonSchema/"
const cli = require('../../../cli/cli')
const CLI_OPTIONS = { typesOnly: false }
const util = require('util')
const writeFile = util.promisify(fs.writeFile)

var jsonObject
var jsonBuffer

const options = {
    target : 'JsonSchema',
}
describe('Test Suite for Basic Types',  () => {

    beforeAll(async () => {
        //Clear generated schema directory
        fs.rmSync(schemaBasePath, {recursive: true, force : true})

        //Create and write IR
        const IR =  await cli.make(basePath, CLI_OPTIONS)
		await writeFile(basePath + 'morphir-ir.json', JSON.stringify(IR))

        // Generate the schema
        await cli.gen(basePath + 'morphir-ir.json', schemaBasePath , options)
        const schemaPath = schemaBasePath + "TestModel.json"

        // Read json into an object
        jsonBuffer = fs.readFileSync(schemaPath, 'utf8')
        jsonObject = JSON.parse(jsonBuffer)

	})

    test('1. Bool type test case', () => {
        const boolSchema = jsonObject["$defs"]["BasicTypes.Paid"]
        const ajv = new Ajv2020()
        const validate = ajv.compile(boolSchema)
        const result = validate(true);
        expect(result).toBe(true)
    })
    test('2. Int type test case', () => {
        const intSchema = jsonObject["$defs"]["BasicTypes.Age"]
        const ajv = new Ajv2020({schemas : [intSchema]})
        const validate = ajv.compile(intSchema)
        const result = validate(45)
        expect(result).toBe(true)
    })
    test('3. Float type test case', () => {
        const floatSchema = jsonObject["$defs"]["BasicTypes.Score"]
        const ajv = new Ajv2020({schemas : [floatSchema]})
        const validate = ajv.compile(floatSchema)
        const result = validate(4.5)
        expect(result).toBe(true)
    })
    test('4. Char type test case', () => {
        const charSchema = jsonObject["$defs"]["BasicTypes.Grade"]
        const ajv = new Ajv2020({schemas : [charSchema]})
        const validate = ajv.compile(charSchema)
        const result = validate('A')
        expect(result).toBe(true)
    })
    test('5. String type test case', () => {
        const stringSchema = jsonObject["$defs"]["BasicTypes.Fullname"]
        const ajv = new Ajv2020({schemas : [stringSchema]})
        const validate = ajv.compile(stringSchema)
        const result = validate("Morphir String")
        expect(result).toBe(true)
    })
})

describe('Test Suite for Advanced Types', () => {
    test('1. Test for Decimal type', () => {
        const decimalSchema = jsonObject["$defs"]["AdvancedTypes.Score"]
        const ajv = new Ajv2020({schemas : [decimalSchema]})
        const validate = ajv.compile(decimalSchema)
        const result = validate("99.9")
        expect(result).toBe(true)
    })

    test.skip('2. Test for LocalDate type', () => {
    })

    test.skip('3. Test for LocalTime type', () => {
    })
    
    test.skip('4. Test for Month type', () => {
    })
})

describe('Test Suite for Optional Types', () => {
    test('Test for MayBe type', () => {
        const mayBeSchema = jsonObject["$defs"]["OptionalTypes.Assignment"]
        const ajv = new Ajv2020({schemas : [mayBeSchema]})
        const validate = ajv.compile(mayBeSchema)
        const result = validate('Bar')
        expect(result).toBe(true)
    })
})

describe('Test Suite for Collection Types', () => {
    test('Test for List type', () => {
        const listSchema = jsonObject["$defs"]["CollectionTypes.Department"]
        const ajv = new Ajv2020({schemas : [listSchema]})
        const validate = ajv.compile(listSchema)
        const result = validate(["HR", "IT", "HR"])
        expect(result).toBe(true)

    })
    test('Test for Set type', () => {
        const setSchema = jsonObject["$defs"]["CollectionTypes.Proids"]
        const ajv = new Ajv2020({schemas : [setSchema]})
        const validate = ajv.compile(setSchema)
        const result = validate(["bsdev", "morphirdev"])
        expect(result).toBe(true)        
    })
    test.skip('Test for Dict type', () => {

    })
})

describe('Test Suite for Result Types', () => {
    test.skip('Test for  type', () => {
    })
})


describe('Test Suite for Composite Types - Records/Tuples', () => {
    test.skip('Test for Tuple  type', () => {
    })

    test('Test for Record type', () => {
        const recordSchema = jsonObject["$defs"]["RecordTypes.Address"]
        const ajv = new Ajv2020({schemas : [recordSchema]})
        const validate = ajv.compile(recordSchema)
        const recordInstance = {
            country : "US",
            state : "New York",
            street : "Devin"
        }
        const result = validate(recordInstance)
        expect(result).toBe(true)
    })

    test.skip('Test for Tuple type', () => {
    })
})

describe('Test Suite for Composite Types - Custom Types', () => {
    test('Test for Enum Type', () => {
        const enumSchema = jsonObject["$defs"]["CustomTypes.Currencies"]
        const ajv = new Ajv2020({schemas : [enumSchema]})
        const validate = ajv.compile(enumSchema)
        const result = validate("USD")
        expect(result).toBe(true)
    })

    test.skip('Test for Custom type 1',  () => {
        const custom1Schema = jsonObject["$defs"]["CustomTypes.Person"]
        const ajv = new Ajv2020({schemas : [custom1Schema]})
        const validate = ajv.compile(custom1Schema)
        const result = validate(["Child", "Bar", 11])
        expect(result).toBe(true)
    })

    test.skip('Test for Custom type 2',  () => {
        const custom2Schema = jsonObject["$defs"]["CustomTypes.Person"]
        const ajv = new Ajv2020({schemas : [custom2Schema]})
        const validate = ajv.compile(custom2Schema)
        const result = validate( ["Adult", "foo"]);
        expect(result).toBe(true)
    })
})