namespace Jsonic
{
    public interface IJsonConverter<T>
    {
        string ToJson(T jsonObject);
    }
}