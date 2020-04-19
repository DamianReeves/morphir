package org.morphir.sdk

import org.morphir.sdk.Maybe.Maybe

import scala.util.matching.{Regex => RE}
import Maybe._

object Regex {

  case class Regex(toRE: RE) extends AnyVal
  case class Options(caseInsensitive: Boolean, multiline: Boolean)
  case class Match(
      `match`: String,
      index: Int,
      number: Int,
      submatches: List[Maybe[String]]
  )

  val never: Regex = Regex(".^".r)
}
