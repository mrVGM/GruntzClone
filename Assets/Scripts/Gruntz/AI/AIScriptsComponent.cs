using Base.Actors;
using ScriptingLanguage.Interpreter;
using ScriptingLanguage.Parser;
using ScriptingLanguage.Tokenizer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.AI
{
    public class AIScriptsComponent : IActorComponent
    {
        class PostData : IFunction
        {
            public Action<string, object> Action;
            const string TagParam = "tag";
            const string DataParam = "data";
            string[] ParamNames = new string[] { TagParam, DataParam };
            public Scope ScopeTemplate
            {
                get
                {
                    var scope = new Scope();
                    scope.AddVariable(TagParam, null);
                    scope.AddVariable(DataParam, null);
                    return scope;
                }
            }

            public string[] ParameterNames => ParamNames;

            public object Execute(Scope scope)
            {
                Action(scope.GetVariable(TagParam) as string, scope.GetVariable(DataParam));
                return null;
            }

            public PostData(Action<string, object> action)
            {
                Action = action;
            }
        }

        class LoadScript : IFunction
        {
            const string filename = "path";
            const string exports = "exports";
            public Scope ScopeTemplate
            {
                get
                {
                    var scope = new Scope { ParentScope = AIScriptsComponent.Session.GetWorkingScope() };
                    scope.AddVariable(filename, null);
                    scope.AddVariable(exports, new GenericObject());

                    var localScope = new Scope { ParentScope = scope };
                    return localScope;
                }
            }
            public string[] ParameterNames { get; private set; } = { filename };
            private AIScriptsComponent AIScriptsComponent { get; }

            public LoadScript(AIScriptsComponent aiScriptsComponent)
            {
                AIScriptsComponent = aiScriptsComponent;
            }

            public object Execute(Scope scope)
            {
                string scriptName = scope.GetVariable(filename) as string;
                string[] path = scriptName.Split('/');

                GameObject script = AIScriptsComponent.Actor.ActorComponent.GetComponentInChildren<AIScriptsBehaviour>().ScriptsRoot;
                for (int i = 0; i < path.Length; ++i) {
                    bool found = false;
                    for (int childIndex = 0; childIndex < script.transform.childCount; ++childIndex) {
                        var child = script.transform.GetChild(childIndex);
                        if (child.name == path[i]) {
                            script = child.gameObject;
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        throw new InvalidOperationException("Can't find AI Script path");
                    }
                }

                var aiScript = script.GetComponent<AIScript>();
                var innerScope = new Scope { ParentScope = scope };
                GenericObject genObject = new GenericObject();

                foreach (var prop in aiScript.Props) {
                    var p = genObject.GetPropoerty(prop.Name);
                    p.ObjectValue = prop.GameObject;
                }

                innerScope.AddVariable("script_props", genObject);
                AIScriptsComponent.Interpreter.RunScript(innerScope, new ScriptId { Filename = scriptName, Script = aiScript.Script });
                return scope.GetVariable("exports");
            }
        }

        public Actor Actor { get; }
        public AIScriptsComponentDef AIScriptsComponentDef;
        public Session Session { get; }
        public Interpreter Interpreter { get; }
        public AIScriptsBehaviour AIScriptsBehaviour => Actor.ActorComponent.GetComponentInChildren<AIScriptsBehaviour>();

        public Dictionary<string, GenericObject> ScriptsMessages = new Dictionary<string, GenericObject>();
        private GenericObject _scriptsData = null;

        public IFunction ScriptsInitFunction
        {
            get
            {
                if (_scriptsData == null) {
                    return null;
                }

                return _scriptsData.GetPropoerty("init").ObjectValue as IFunction;
            }
        }

        public IFunction ScriptsUpdateFunction
        {
            get
            {
                if (_scriptsData == null) {
                    return null;
                }

                return _scriptsData.GetPropoerty("update").ObjectValue as IFunction;
            }
        }

        public void ExecuteScriptsInitFunction()
        {
            if (ScriptsInitFunction == null) {
                return;
            }

            var scope = ScriptsInitFunction.ScopeTemplate;
            scope.SetVariable("actor", Actor);
            ScriptsInitFunction.Execute(scope);
        }

        public void ExecuteScriptsUpdateFunction(Actor possessedActor)
        {
            if (ScriptsUpdateFunction == null) {
                return;
            }

            var scope = ScriptsUpdateFunction.ScopeTemplate;
            scope.SetVariable("possessed_actor", possessedActor);
            ScriptsUpdateFunction.Execute(scope);
        }

        public AIScriptsComponent(Actor actor, AIScriptsComponentDef aiScriptsComponentDef)
        {
            Actor = actor;
            AIScriptsComponentDef = aiScriptsComponentDef;

            var parserTable = ParserTable.Deserialize(AIScriptsComponentDef.ParserTable.bytes);
            var parser = new Parser { ParserTable = parserTable };

            
            void receiveData(string tag, object data)
            {
                ScriptsMessages[tag] = data as GenericObject;
            }

            var postData = new PostData(receiveData);
            Session = new Session("", parser,
                new Session.SessionFunc { Name = "post_object", Function = postData },
                new Session.SessionFunc { Name = "load_script", Function = new LoadScript(this)});
            Interpreter = new Interpreter(Session);


            var scriptId = new ScriptId { Filename = "", Script = AIScriptsBehaviour.Script };
            Interpreter.RunScript(Session.GetWorkingScope(), scriptId);

            ScriptsMessages.TryGetValue("funcs", out _scriptsData);
            ScriptsMessages.Clear();
        }

        public void DeInit()
        {
        }

        public void Init()
        {
        }
    }
}
