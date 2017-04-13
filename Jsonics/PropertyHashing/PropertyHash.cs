namespace Jsonics.PropertyHashing
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