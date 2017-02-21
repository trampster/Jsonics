namespace Jsonics
{
    public interface IJsonConverter<T>
    {
        string ToJson(T jsonObject);
    }
}