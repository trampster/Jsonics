# Jsonics
High performance Json Library for c#. Jsonics stands for JSON In C Sharp. Jsonics aims to be as fast as is possible in scenarios where the same type will be serialized or deserialized many times. Jsonics uses runtime code generation and other advanced techniques to create an optimial serializer and deserializer based on the supplied type.

## Usage
    //create an optimized Json converter for type Person
    var jsonConverter = JsonicFactory.Compile<Person>();
    
    //serilize a person instance
    string jsonstring = jsonCoverter.ToJson(new Person(){FirstName="Luke", LastName="Skywalker"});
    
    //deserialize a person json string
    Person person = jsonCoverter.ToJson(jsonString);



