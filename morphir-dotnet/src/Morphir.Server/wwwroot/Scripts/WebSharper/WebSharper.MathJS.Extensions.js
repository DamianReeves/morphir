(function(Global)
{
 "use strict";
 var WebSharper,Decimal,SC$1,Arrays,Operators,math;
 WebSharper=Global.WebSharper=Global.WebSharper||{};
 Decimal=WebSharper.Decimal=WebSharper.Decimal||{};
 SC$1=Global.StartupCode$WebSharper_MathJS_Extensions$Decimal=Global.StartupCode$WebSharper_MathJS_Extensions$Decimal||{};
 Arrays=WebSharper&&WebSharper.Arrays;
 Operators=WebSharper&&WebSharper.Operators;
 math=Global.math;
 Decimal.CreateDecimalBits=function(bits)
 {
  var sign,scale;
  return Arrays.length(bits)===4?(sign=(Arrays.get(bits,3)&-2147483648)!==0,(scale=Arrays.get(bits,3)>>16&127,Decimal.CreateDecimal(Arrays.get(bits,0),Arrays.get(bits,1),Arrays.get(bits,2),sign,scale))):Operators.InvalidArg("bits","The length of the bits array is not 4");
 };
 Decimal.CreateDecimal=function(lo,mid,hi,isNegative,scale)
 {
  var uint_sup,a,a$1,a$2,_this,a$3,_this$1,quotient,value,a$4,a$5,a$6,a$7,_this$2,a$8,_this$3,_this$4,a$9,_this$5,sign,a$10,_this$6,_this$7;
  function n(x)
  {
   return Decimal.WSDecimalMath().bignumber(x);
  }
  function reinterpret(x)
  {
   var a$11,_this$8;
   return x>=0?n(x):(a$11=n(x),(_this$8=Decimal.WSDecimalMath(),_this$8.add.apply(_this$8,[uint_sup,a$11])));
  }
  return lo===0&&hi===0&&mid===0?n(0):(uint_sup=(a=(a$1=n(429496729),(a$2=n(10),(_this=Decimal.WSDecimalMath(),_this.multiply.apply(_this,[a$1,a$2])))),(a$3=n(6),(_this$1=Decimal.WSDecimalMath(),_this$1.add.apply(_this$1,[a,a$3])))),(quotient=Decimal.WSDecimalMath().pow(n(10),Decimal.WSDecimalMath().unaryMinus(n(Operators.toInt(scale)))),(value=(a$4=(a$5=(a$6=(a$7=reinterpret(hi),(_this$2=Decimal.WSDecimalMath(),_this$2.multiply.apply(_this$2,[a$7,uint_sup]))),(a$8=reinterpret(mid),(_this$3=Decimal.WSDecimalMath(),_this$3.add.apply(_this$3,[a$6,a$8])))),(_this$4=Decimal.WSDecimalMath(),_this$4.multiply.apply(_this$4,[a$5,uint_sup]))),(a$9=reinterpret(lo),(_this$5=Decimal.WSDecimalMath(),_this$5.add.apply(_this$5,[a$4,a$9])))),(sign=isNegative?n(-1):n(1),(a$10=(_this$6=Decimal.WSDecimalMath(),_this$6.multiply.apply(_this$6,[sign,value])),(_this$7=Decimal.WSDecimalMath(),_this$7.multiply.apply(_this$7,[a$10,quotient])))))));
 };
 Decimal.WSDecimalMath=function()
 {
  SC$1.$cctor();
  return SC$1.WSDecimalMath;
 };
 SC$1.$cctor=function()
 {
  var r;
  SC$1.$cctor=Global.ignore;
  SC$1.WSDecimalMath=math.create((r={},r.number="BigNumber",r.precision=29,r.predictable=true,r.epsilon=1E-60,r));
 };
}(self));
