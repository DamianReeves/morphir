(function(Global)
{
 "use strict";
 var WebSharper,UI,Templating,Runtime,Server,TemplateInitializer,Obj,TemplateInstances,Handler,ProviderBuilder,TemplateInstance,Client,ClientTemplateInstanceHandlers,Collections,Dictionary,Runtime$1,Arrays,Unchecked,TemplateHoleModule,VarStr,Var$1,VarFloatUnchecked,VarBool,VarDateTime,VarFile,VarDomElement,Operators,Doc,Client$1,Templates,Slice,View,Activator,HashSet,Enumerator,Seq,AfterRenderQ,EventQ,System,Guid;
 WebSharper=Global.WebSharper=Global.WebSharper||{};
 UI=WebSharper.UI=WebSharper.UI||{};
 Templating=UI.Templating=UI.Templating||{};
 Runtime=Templating.Runtime=Templating.Runtime||{};
 Server=Runtime.Server=Runtime.Server||{};
 TemplateInitializer=Server.TemplateInitializer=Server.TemplateInitializer||{};
 Obj=WebSharper&&WebSharper.Obj;
 TemplateInstances=Server.TemplateInstances=Server.TemplateInstances||{};
 Handler=Server.Handler=Server.Handler||{};
 ProviderBuilder=Server.ProviderBuilder=Server.ProviderBuilder||{};
 TemplateInstance=Server.TemplateInstance=Server.TemplateInstance||{};
 Client=Runtime.Client=Runtime.Client||{};
 ClientTemplateInstanceHandlers=Client.ClientTemplateInstanceHandlers=Client.ClientTemplateInstanceHandlers||{};
 Collections=WebSharper&&WebSharper.Collections;
 Dictionary=Collections&&Collections.Dictionary;
 Runtime$1=WebSharper&&WebSharper.Runtime;
 Arrays=WebSharper&&WebSharper.Arrays;
 Unchecked=WebSharper&&WebSharper.Unchecked;
 TemplateHoleModule=UI&&UI.TemplateHoleModule;
 VarStr=TemplateHoleModule&&TemplateHoleModule.VarStr;
 Var$1=UI&&UI.Var$1;
 VarFloatUnchecked=TemplateHoleModule&&TemplateHoleModule.VarFloatUnchecked;
 VarBool=TemplateHoleModule&&TemplateHoleModule.VarBool;
 VarDateTime=TemplateHoleModule&&TemplateHoleModule.VarDateTime;
 VarFile=TemplateHoleModule&&TemplateHoleModule.VarFile;
 VarDomElement=TemplateHoleModule&&TemplateHoleModule.VarDomElement;
 Operators=WebSharper&&WebSharper.Operators;
 Doc=UI&&UI.Doc;
 Client$1=UI&&UI.Client;
 Templates=Client$1&&Client$1.Templates;
 Slice=WebSharper&&WebSharper.Slice;
 View=UI&&UI.View;
 Activator=WebSharper&&WebSharper.Activator;
 HashSet=Collections&&Collections.HashSet;
 Enumerator=WebSharper&&WebSharper.Enumerator;
 Seq=WebSharper&&WebSharper.Seq;
 AfterRenderQ=TemplateHoleModule&&TemplateHoleModule.AfterRenderQ;
 EventQ=TemplateHoleModule&&TemplateHoleModule.EventQ;
 System=Global.System;
 Guid=System&&System.Guid;
 TemplateInitializer.$cctor=function()
 {
  TemplateInitializer.$cctor=Global.ignore;
  TemplateInitializer.initialized=new Dictionary.New$5();
 };
 TemplateInitializer=Server.TemplateInitializer=Runtime$1.Class({
  get_Vars:function()
  {
   TemplateInitializer.$cctor();
   return this.vars;
  },
  get_Id:function()
  {
   TemplateInitializer.$cctor();
   return this.id;
  },
  InitInstance:function(key)
  {
   var $1,d,a,i,$2;
   TemplateInitializer.$cctor();
   d=TemplateInitializer.GetHolesFor(key);
   a=this.vars;
   for(i=0,$2=a.length-1;i<=$2;i++)(function()
   {
    var f,t,ov,n,o,o$1,o$2,o$3,o$4,x,v;
    f=Arrays.get(a,i);
    t=f[1];
    ov=f[2];
    n=f[0];
    return!d.ContainsKey(n)?d.set_Item(n,Unchecked.Equals(t,0)?new VarStr.New(n,Var$1.Create$1((o=ov==null?null:{
     $:1,
     $0:ov.$0
    },o==null?"":o.$0))):Unchecked.Equals(t,1)?new VarFloatUnchecked.New(n,Var$1.Create$1((o$1=ov==null?null:{
     $:1,
     $0:ov.$0
    },o$1==null?0:o$1.$0))):Unchecked.Equals(t,2)?new VarBool.New(n,Var$1.Create$1((o$2=ov==null?null:{
     $:1,
     $0:ov.$0
    },o$2==null?false:o$2.$0))):Unchecked.Equals(t,3)?new VarDateTime.New(n,Var$1.Create$1((o$3=ov==null?null:{
     $:1,
     $0:ov.$0
    },o$3==null?-8640000000000000:o$3.$0))):Unchecked.Equals(t,4)?new VarFile.New(n,Var$1.Create$1((o$4=ov==null?null:{
     $:1,
     $0:ov.$0
    },o$4==null?[]:o$4.$0))):Unchecked.Equals(t,5)?new VarDomElement.New(n,Var$1.Create$1({
     $:1,
     $0:(x=ov==null?null:{
      $:1,
      $0:ov.$0
     },(v=self.document.querySelector("[ws-dom="+n+"]"),x==null?v:x.$0))
    })):Operators.FailWith("Invalid value type")):null;
   }());
   this.instance=new TemplateInstance.New({
    $:0,
    $0:d
   },Doc.get_Empty());
  },
  get_Instance:function()
  {
   TemplateInitializer.$cctor();
   return Runtime$1.GetOptional(this.instance).$0;
  },
  $postinit:function(key)
  {
   TemplateInitializer.$cctor();
   Templates.RunFullDocTemplate([]);
  },
  $init:function(a)
  {
   TemplateInitializer.$cctor();
  },
  $preinit:function(key)
  {
   var q,i,$1,el,fullName,s,hole,hole$1;
   TemplateInitializer.$cctor();
   this.InitInstance(key);
   q=self.document.querySelectorAll("[ws-var^='"+key+"::']");
   for(i=0,$1=q.length-1;i<=$1;i++){
    el=q[i];
    fullName=el.getAttribute("ws-var");
    s=Slice.string(fullName,{
     $:1,
     $0:key.length+2
    },null);
    hole=this.get_Instance().Hole(s);
    hole$1=hole.WithName(fullName);
    Templates.GlobalHoles().set_Item(hole$1.get_Name(),hole$1);
    TemplateInitializer.applyVarHole(el,hole);
   }
  }
 },Obj,TemplateInitializer);
 TemplateInitializer.applyVarHole=function(el,tpl)
 {
  TemplateInitializer.$cctor();
  tpl.ApplyVarHole(el);
 };
 TemplateInitializer.applyTypedVarHole=function(bind,v,el)
 {
  var p;
  TemplateInitializer.$cctor();
  p=bind(v);
  p[0](el);
  View.Sink(p[1](el),p[2]);
 };
 TemplateInitializer.GetOrAddHoleFor=function(id,holeName,initHole)
 {
  var d,m,o,h;
  TemplateInitializer.$cctor();
  d=TemplateInitializer.GetHolesFor(id);
  m=(o=null,[d.TryGetValue(holeName,{
   get:function()
   {
    return o;
   },
   set:function(v)
   {
    o=v;
   }
  }),o]);
  return m[0]?m[1]:(h=initHole(),(d.set_Item(holeName,h),h));
 };
 TemplateInitializer.GetHolesFor=function(id)
 {
  var m,o,d;
  TemplateInitializer.$cctor();
  m=(o=null,[TemplateInitializer.initialized.TryGetValue(id,{
   get:function()
   {
    return o;
   },
   set:function(v)
   {
    o=v;
   }
  }),o]);
  return m[0]?m[1]:(d=new Dictionary.New$5(),(TemplateInitializer.initialized.set_Item(id,d),d));
 };
 TemplateInitializer.get_Initialized=function()
 {
  TemplateInitializer.$cctor();
  return TemplateInitializer.initialized;
 };
 TemplateInitializer.New=Runtime$1.Ctor(function(id,vars)
 {
  TemplateInitializer.$cctor();
  Obj.New.call(this);
  this.vars=vars;
  Runtime$1.SetOptional(this,"instance",null);
  this.id=id;
 },TemplateInitializer);
 TemplateInstances=Server.TemplateInstances=Runtime$1.Class({},Obj,TemplateInstances);
 TemplateInstances.GetInstance=function(key)
 {
  return(Activator.Instances())[key].get_Instance();
 };
 TemplateInstances.New=Runtime$1.Ctor(function()
 {
  Obj.New.call(this);
 },TemplateInstances);
 Handler.AfterRenderQ2$204$42=function(key,f)
 {
  return function(el)
  {
   var i;
   i=TemplateInstances.GetInstance(key);
   i.SetAnchorRoot(el);
   f({
    Vars:i,
    Anchors:i,
    Target:el,
    Event:null
   });
  };
 };
 Handler.EventQ2$188$36=function(key,f)
 {
  return function(el)
  {
   return function(ev)
   {
    var i;
    i=TemplateInstances.GetInstance(key);
    i.SetAnchorRoot(el);
    return f({
     Vars:i,
     Anchors:i,
     Target:el,
     Event:ev
    });
   };
  };
 };
 Handler.CompleteHoles=function(key,filledHoles,vars)
 {
  var allVars,filledVars,e,h,n;
  function c(name,ty,a)
  {
   var r;
   return filledVars.Contains(name)?null:(r=Unchecked.Equals(ty,0)?TemplateInitializer.GetOrAddHoleFor(key,name,function()
   {
    return new VarStr.New(name,Var$1.Create$1(""));
   }):Unchecked.Equals(ty,1)?TemplateInitializer.GetOrAddHoleFor(key,name,function()
   {
    return new VarFloatUnchecked.New(name,Var$1.Create$1(0));
   }):Unchecked.Equals(ty,2)?TemplateInitializer.GetOrAddHoleFor(key,name,function()
   {
    return new VarBool.New(name,Var$1.Create$1(false));
   }):Unchecked.Equals(ty,3)?TemplateInitializer.GetOrAddHoleFor(key,name,function()
   {
    return new VarDateTime.New(name,Var$1.Create$1(-8640000000000000));
   }):Unchecked.Equals(ty,4)?TemplateInitializer.GetOrAddHoleFor(key,name,function()
   {
    return new VarFile.New(name,Var$1.Create$1([]));
   }):Unchecked.Equals(ty,5)?TemplateInitializer.GetOrAddHoleFor(key,name,function()
   {
    var el;
    el=self.document.querySelector("[ws-dom="+name+"]");
    el.removeAttribute("ws-dom");
    return new VarDomElement.New(name,Var$1.Create$1({
     $:1,
     $0:el
    }));
   }):Operators.FailWith("Invalid value type"),(allVars.set_Item(name,r),{
    $:1,
    $0:r
   }));
  }
  allVars=new Dictionary.New$5();
  filledVars=new HashSet.New$3();
  e=Enumerator.Get(filledHoles);
  try
  {
   while(e.MoveNext())
    {
     h=e.Current();
     n=h.get_Name();
     filledVars.SAdd(n);
     allVars.set_Item(n,h);
    }
  }
  finally
  {
   if(typeof e=="object"&&"Dispose"in e)
    e.Dispose();
  }
  return[Seq.append(filledHoles,Arrays.choose(function($1)
  {
   return c($1[0],$1[1],$1[2]);
  },vars)),{
   $:0,
   $0:allVars
  }];
 };
 Handler.AfterRenderQ2=function(key,holeName,ti,f)
 {
  return new AfterRenderQ.New(holeName,function(el)
  {
   var i;
   i=ti();
   i.SetAnchorRoot(el);
   f({
    Vars:i,
    Anchors:i,
    Target:el,
    Event:null
   });
  });
 };
 Handler.EventQ2=function(key,holeName,ti,f)
 {
  return new EventQ.New(holeName,function(el)
  {
   return function(ev)
   {
    var i;
    i=ti();
    i.SetAnchorRoot(el);
    return f({
     Vars:i,
     Anchors:i,
     Target:el,
     Event:ev
    });
   };
  });
 };
 ProviderBuilder=Server.ProviderBuilder=Runtime$1.Class({},Obj,ProviderBuilder);
 ProviderBuilder.New=Runtime$1.Ctor(function(src)
 {
  var c;
  Obj.New.call(this);
  this.i=null;
  this.k=(c=Guid.NewGuid(),Global.String(c));
  this.h=Runtime$1.MarkResizable([]);
  this.s=src;
 },ProviderBuilder);
 ProviderBuilder.New$1=Runtime$1.Ctor(function()
 {
  var c;
  Obj.New.call(this);
  this.i=null;
  this.k=(c=Guid.NewGuid(),Global.String(c));
  this.h=Runtime$1.MarkResizable([]);
  Runtime$1.SetOptional(this,"s",null);
 },ProviderBuilder);
 TemplateInstance=Server.TemplateInstance=Runtime$1.Class({
  Anchor:function(name)
  {
   function findUnder(el)
   {
    var e;
    while(true)
     {
      e=el.querySelector("[ws-anchor=\""+Global.String(name)+"\"]");
      if(e==null&&!(el.parentElement==null))
       el=el.parentElement;
      else
       return e;
     }
   }
   return findUnder(this.anchorRoot);
  },
  SetAnchorRoot:function(el)
  {
   this.anchorRoot=el;
  },
  Hole:function(name)
  {
   return this.allVars.Item(name);
  },
  get_Doc:function()
  {
   return this.doc;
  }
 },Obj,TemplateInstance);
 TemplateInstance.New=Runtime$1.Ctor(function(c,doc)
 {
  Obj.New.call(this);
  this.doc=doc;
  this.allVars=c.$==0?c.$0:Operators.FailWith("Should not happen");
  this.anchorRoot=null;
 },TemplateInstance);
 ClientTemplateInstanceHandlers.AfterRenderQ2Client=function(key,el,f)
 {
  var i;
  i=TemplateInstances.GetInstance(key);
  i.SetAnchorRoot(el);
  f({
   Vars:i,
   Anchors:i,
   Target:el,
   Event:null
  });
 };
 ClientTemplateInstanceHandlers.EventQ2Client=function(key,el,ev,f)
 {
  var i;
  i=TemplateInstances.GetInstance(key);
  i.SetAnchorRoot(el);
  f({
   Vars:i,
   Anchors:i,
   Target:el,
   Event:ev
  });
 };
 Client.AfterRenderQ2$71$26=function(f)
 {
  return function()
  {
   f();
  };
 };
 Client.Box=Global.id;
}(self));
