# Jsonics
High performance Json Library for c#. Jsonics stands for JSON In C Sharp. Jsonics aims to be a fast as is possible in scenarios where the same type will be serialized or deserialized many times. If you are going to use a type only once and performance is critical then use something else.

## Usage
    //create an optimized Json converter for type Person
    var jsonConverter = JsonicFactory.Compile<Person>();
    
    //serilize a person instance
    string jsonstring = jsonCoverter.ToJson(new Person(){FirstName="Luke", LastName="Skywalker"});
    
    //deserialize a person json string
    Person person = jsonCoverter.ToJson(jsonString);



