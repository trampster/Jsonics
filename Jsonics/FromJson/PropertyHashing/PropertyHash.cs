using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.FromJson.PropertyHashing
{
    internal class PropertyHash
    {
        internal int Column {get;set;}
        internal bool UseLength{get;set;}
        internal int ModValue{get;set;}
        internal int CollisionCount {get;set;}

        internal int Hash(string property)
        {
            if(UseLength) 
            {
                return property.Length % ModValue;
            }
            return property[Column % property.Length] % ModValue;
        }

        internal LocalBuilder EmitHash(JsonILGenerator generator, LocalBuilder propertyNameLocal)
        {
            var hashLocal = generator.DeclareLocal<int>();
            if(UseLength)
            {
                generator.LoadLocalAddress(propertyNameLocal);
                generator.Call(typeof(LazyString).GetRuntimeMethod("get_Length", new Type[0]));
                generator.LoadConstantInt32(this.ModValue);
                generator.Remainder();
                generator.StoreLocal(hashLocal);
                return hashLocal;
            }
            generator.LoadLocalAddress(propertyNameLocal);
            generator.LoadConstantInt32(this.Column);
            generator.LoadLocalAddress(propertyNameLocal);
            generator.Call(typeof(LazyString).GetRuntimeMethod("get_Length", new Type[0]));
            generator.Remainder();
            generator.Call(typeof(LazyString).GetRuntimeMethod("At", new Type[]{typeof(int)}));
            generator.LoadConstantInt32(this.ModValue);
            generator.Remainder();
            generator.StoreLocal(hashLocal);
            return hashLocal;
        }

        internal bool IsBetterHash(PropertyHash otherHash)
        {
            if(otherHash.CollisionCount < CollisionCount)
            {
                return true;
            }
            if(otherHash.CollisionCount > CollisionCount)
            {
                return false;
            }
            //same number collisions, use the one with the smallest mod
            if(otherHash.ModValue > ModValue)
            {
                return false;
            }
            return true;
        }
    }
}