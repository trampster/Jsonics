using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.FromJson.PropertyHashing
{
    public class PropertyHash
    {
        public int Column {get;set;}
        public bool UseLength{get;set;}
        public int ModValue{get;set;}
        public int CollisionCount {get;set;}

        public int Hash(string property)
        {
            if(UseLength) 
            {
                return property.Length % ModValue;
            }
            return property[Column % property.Length] % ModValue;
        }

        public LocalBuilder EmitHash(JsonILGenerator generator, LocalBuilder propertyNameLocal)
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

        public bool IsBetterHash(PropertyHash otherHash)
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