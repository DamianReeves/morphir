import mill._, mill.scalalib._, mill.scalajslib._, mill.scalanativelib._, scalafmt._

val crossScalaVersions = Seq("3.3.1", "2.13.11")

object sdk extends Cross[SdkModule](crossScalaVersions)
trait SdkModule extends Cross.Module[String]  {

    object core extends ScalaModule {
        def scalaVersion = crossValue
    }
}