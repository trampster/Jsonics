using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System;

namespace Jsonics
{
    public class JsonILGenerator
    {
        StringBuilder _appendQueue;
        ILGenerator _generator; 

        public JsonILGenerator(ILGenerator generator, StringBuilder appendQueue)
        {
            _generator = generator;
            _appendQueue = appendQueue;
        }

        public StringBuilder AppendQueue
        {
            get
            {
                return _appendQueue;
            }
        }

        public void Append(string value)
        {
            _appendQueue.Append(value);
        }

        public void EmitQueuedAppends()
        {
            if(_appendQueue.Length == 0)
            {
                return;
            }
            if(_appendQueue.Length == 1)
            {
                _generator.Emit(OpCodes.Ldc_I4_S, (int)_appendQueue.ToString()[0]);
                EmitAppend(typeof(char));
                _appendQueue.Clear();
                return;
            }
            _generator.Emit(OpCodes.Ldstr, _appendQueue.ToString());
            EmitAppend(typeof(string));
            _appendQueue.Clear();
        }

        public void EmitAppend(Type type)
        {
            _generator.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetRuntimeMethod("Append", new Type[]{type}));
        }

        public LocalBuilder DeclareLocal<T>()
        {
            return _generator.DeclareLocal(typeof(T));
        }

        public LocalBuilder DeclareLocal(Type type)
        {
            return _generator.DeclareLocal(type);
        }

        public void StoreLocal(LocalBuilder localBuilder)
        {
            EmitQueuedAppends();
            int index = localBuilder.LocalIndex;
            if(index == 0)
            {
                _generator.Emit(OpCodes.Stloc_0);
                return;
            }
            if(index == 1)
            {
                _generator.Emit(OpCodes.Stloc_1);
                return;
            }
            if(index == 2)
            {
                _generator.Emit(OpCodes.Stloc_2);
                return;
            }
            if(index == 3)
            {
                _generator.Emit(OpCodes.Stloc_3);
                return;
            }
            if(index <= 255)
            {
                _generator.Emit(OpCodes.Stloc_S, localBuilder);
                return;
            }
            _generator.Emit(OpCodes.Stloc, localBuilder);
        }

        public void LoadLocal(LocalBuilder localBuilder)
        {
            EmitQueuedAppends();
            int index = localBuilder.LocalIndex;
            if(index == 0)
            {
                _generator.Emit(OpCodes.Ldloc_0);
                return;
            }
            if(index == 1)
            {
                _generator.Emit(OpCodes.Ldloc_1);
                return;
            }
            if(index == 2)
            {
                _generator.Emit(OpCodes.Ldloc_2);
                return;
            }
            if(index == 3)
            {
                _generator.Emit(OpCodes.Ldloc_3);
                return;
            }
            if(index <= 255)
            {
                _generator.Emit(OpCodes.Ldloc_S, localBuilder);
                return;
            }
            _generator.Emit(OpCodes.Ldloc, localBuilder);
        }

        public void LoadLocalAddress(LocalBuilder localBuilder)
        {
            EmitQueuedAppends();
            if(localBuilder.LocalIndex <= 255)
            {
                _generator.Emit(OpCodes.Ldloca_S, localBuilder);
                return;
            }
            _generator.Emit(OpCodes.Ldloca, localBuilder);
        }

        public void Constrain<T>()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Constrained, typeof(T));
        }

        public void Constrain(Type type)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Constrained, type);
        }

        public void CallToString()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Callvirt, typeof(Object).GetRuntimeMethod("ToString", new Type[0]));
        }

        public void LoadArg(Type type, int arg)
        {
            EmitQueuedAppends();

            if(type.GetTypeInfo().IsValueType && !type.GetTypeInfo().IsPrimitive && type != typeof(DateTime) && type != typeof(Guid) && !type.GetTypeInfo().IsEnum)
            {
                _generator.Emit(OpCodes.Ldarga_S, arg);
                return;
            }

            if(arg == 0)
            {
                _generator.Emit(OpCodes.Ldarg_0);
                return;
            }
            if(arg == 1)
            {
                _generator.Emit(OpCodes.Ldarg_1);
                return;
            }
            if(arg == 2)
            {
                _generator.Emit(OpCodes.Ldarg_2);
                return;
            }
            if(arg == 3)
            {
                _generator.Emit(OpCodes.Ldarg_3);
                return;
            }
            _generator.Emit(OpCodes.Ldarg_S, (byte)arg);
            return;
        }

        public void GetProperty(PropertyInfo property)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Call, property.DeclaringType.GetRuntimeMethod($"get_{property.Name}", new Type[0]));
        }

        public void AppendDate()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Call, typeof(StringBuilderExtension).GetRuntimeMethod("AppendDate", new Type[]{typeof(StringBuilder), typeof(DateTime)}));
        }
        
        public Label DefineLabel()
        {
            return _generator.DefineLabel();
        }

        public void Mark(Label label)
        {
            _generator.MarkLabel(label);
        }

        public void BrIfTrue(Label label)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Brtrue, label);
        }

        public void BranchIfFalse(Label label)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Brfalse, label);
        }

        public void LoadString(string value)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Ldstr, value);
        }

        public void Branch(Label label)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Br, label);
        }

        public void AppendInt()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Call, typeof(StringBuilderExtension).GetRuntimeMethod("AppendInt", new Type[]{typeof(StringBuilder), typeof(int)}));
        }

        public void EmitAppendEscaped()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Call, typeof(StringBuilderExtension).GetRuntimeMethod("AppendEscaped", new Type[]{typeof(StringBuilder), typeof(string)}));
        }

        public void Return()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Ret);
        }

        public void LoadStaticField(FieldBuilder fieldBuilder)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Ldsfld, fieldBuilder);
        }

        public void LoadField(FieldInfo fieldInfo)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Ldfld, fieldInfo);
        }

        public void StoreStaticField(FieldBuilder fieldBuilder)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Stsfld, fieldBuilder);
        }

        public void NewObject(ConstructorInfo constructorInfo)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Newobj, constructorInfo);
        }

        public void CallVirtual(MethodInfo methodInfo)
        {
            EmitQueuedAppends();
            _generator.EmitCall(OpCodes.Callvirt, methodInfo, null);            
        }

        public void Call(MethodInfo methodInfo)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Call, methodInfo);            
        }

        public void Call(ConstructorInfo constructorInfo)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Call, constructorInfo);
        }

        public void Duplicate()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Dup);
        }

        public void LoadLength()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Ldlen);            
        }

        public void ConvertToInt32()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Conv_I4);            
        }

        public void LoadConstantInt32(int val)
        {
            EmitQueuedAppends();
            if(val == 0)
            {
                _generator.Emit(OpCodes.Ldc_I4_0);
                return;
            }
            if(val == 1)
            {
                _generator.Emit(OpCodes.Ldc_I4_1);
                return;
            }
            if(val == 2)
            {
                _generator.Emit(OpCodes.Ldc_I4_2);
                return;
            }
            if(val == 3)
            {
                _generator.Emit(OpCodes.Ldc_I4_3);
                return;
            }
            if(val == 4)
            {
                _generator.Emit(OpCodes.Ldc_I4_4);
                return;
            }
            if(val == 5)
            {
                _generator.Emit(OpCodes.Ldc_I4_5);
                return;
            }
            if(val == 6)
            {
                _generator.Emit(OpCodes.Ldc_I4_6);
                return;
            }
            if(val == 7)
            {
                _generator.Emit(OpCodes.Ldc_I4_7);
                return;
            }
            if(val == 8)
            {
                _generator.Emit(OpCodes.Ldc_I4_8);
                return;
            }
            if(val == -1)
            {
                _generator.Emit(OpCodes.Ldc_I4_M1);
                return;
            }
            if((val > 8 && val < 128) || (val < -1 && val > -129))
            {
                _generator.Emit(OpCodes.Ldc_I4_S, (byte)val);
                return;
            }
            _generator.Emit(OpCodes.Ldc_I4, val);
        }

        public void BranchIfLargerThan(Label label)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Blt, label);
        }

        public void BranchIfEqual(Label label)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Beq, label);
        }

        public void BranchIfNotEqualUnsigned(Label label)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Bne_Un, label);
        }

        public void Pop()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Pop);
        }

        public void LoadArrayElement(Type elementType)
        {
            EmitQueuedAppends();
            if(elementType == typeof(int))
            {
                _generator.Emit(OpCodes.Ldelem_I4);
                return;
            }
            if(elementType.GetTypeInfo().IsValueType)
            {
                _generator.Emit(OpCodes.Ldelem);
                return;
            }
            _generator.Emit(OpCodes.Ldelem_Ref);
        }

        public void LoadListElement(Type listType)
        {
            CallVirtual(listType.GetRuntimeMethod("get_Item", new Type[]{typeof(int)}));
        }

        public void Add()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Add);
        }

        public void Subtract()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Sub);
        }

        public void WriteConsoleInt()
        {
            EmitQueuedAppends();
            var temp = _generator.DeclareLocal(typeof(int));
            _generator.Emit(OpCodes.Stloc, temp);
            _generator.EmitWriteLine(temp);
        }

        public void WriteConsole(string str)
        {
            _generator.EmitWriteLine(str);
        }

        public void WriteConsole(LocalBuilder localBuilder)
        {
            _generator.EmitWriteLine(localBuilder);
        }

        public void Remainder()
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Rem);
        }

        public void Switch(Label[] labels)
        {
            EmitQueuedAppends();
            _generator.Emit(OpCodes.Switch, labels);          
        }

    }
}