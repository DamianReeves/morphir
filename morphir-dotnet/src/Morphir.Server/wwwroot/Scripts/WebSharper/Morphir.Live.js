(function(Global)
{
 "use strict";
 var Morphir,Live,Client,Morphir$Live_Templates,WebSharper,Concurrency,Remoting,AjaxRemotingProvider,UI,TemplateHole,Var$1,Templating,Runtime,Server,ProviderBuilder,Handler,TemplateHoleModule,TextView,TemplateInstance,Client$1,Templates;
 Morphir=Global.Morphir=Global.Morphir||{};
 Live=Morphir.Live=Morphir.Live||{};
 Client=Live.Client=Live.Client||{};
 Morphir$Live_Templates=Global.Morphir$Live_Templates=Global.Morphir$Live_Templates||{};
 WebSharper=Global.WebSharper;
 Concurrency=WebSharper&&WebSharper.Concurrency;
 Remoting=WebSharper&&WebSharper.Remoting;
 AjaxRemotingProvider=Remoting&&Remoting.AjaxRemotingProvider;
 UI=WebSharper&&WebSharper.UI;
 TemplateHole=UI&&UI.TemplateHole;
 Var$1=UI&&UI.Var$1;
 Templating=UI&&UI.Templating;
 Runtime=Templating&&Templating.Runtime;
 Server=Runtime&&Runtime.Server;
 ProviderBuilder=Server&&Server.ProviderBuilder;
 Handler=Server&&Server.Handler;
 TemplateHoleModule=UI&&UI.TemplateHoleModule;
 TextView=TemplateHoleModule&&TemplateHoleModule.TextView;
 TemplateInstance=Server&&Server.TemplateInstance;
 Client$1=UI&&UI.Client;
 Templates=Client$1&&Client$1.Templates;
 Client.Main$19$20=function(rvReversed)
 {
  return function(e)
  {
   var _;
   Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
   {
    return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("Morphir.Live:Morphir.Live.Server.DoSomething:14225411",[TemplateHole.Value(e.Vars.Hole("texttoreverse")).Get()]),function(a)
    {
     rvReversed.Set(a);
     return Concurrency.Zero();
    });
   })),null);
  };
 };
 Client.Main=function()
 {
  var rvReversed,b,R,_this,t,p,i;
  rvReversed=Var$1.Create$1("");
  return(b=(R=rvReversed.get_View(),(_this=(t=new ProviderBuilder.New$1(),(t.h.push(Handler.EventQ2(t.k,"onsend",function()
  {
   return t.i;
  },function(e)
  {
   var _;
   Concurrency.StartImmediate((_=null,Concurrency.Delay(function()
   {
    return Concurrency.Bind((new AjaxRemotingProvider.New()).Async("Morphir.Live:Morphir.Live.Server.DoSomething:14225411",[TemplateHole.Value(e.Vars.Hole("texttoreverse")).Get()]),function(a)
    {
     rvReversed.Set(a);
     return Concurrency.Zero();
    });
   })),null);
  })),t)),(_this.h.push(new TextView.New("reversed",R)),_this))),(p=Handler.CompleteHoles(b.k,b.h,[["texttoreverse",0,null]]),(i=new TemplateInstance.New(p[1],Morphir$Live_Templates.mainform(p[0])),b.i=i,i))).get_Doc();
 };
 Morphir$Live_Templates.mainform=function(h)
 {
  Templates.LoadLocalTemplates("main");
  return h?Templates.NamedTemplate("main",{
   $:1,
   $0:"mainform"
  },h):void 0;
 };
}(self));
