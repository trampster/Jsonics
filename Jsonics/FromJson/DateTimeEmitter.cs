using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonics.FromJson
{
    public class DateTimeEmitter : FromJsonEmitter
    {
        public DateTimeEmitter(LocalBuilder lazyStringLocal, JsonILGenerator generator, FromJsonEmitters emitters)
            : base(lazyStringLocal, generator, emitters)
        {
        }
        
        public override void Emit(LocalBuilder indexLocal, Type type)
        {
            _generator.LoadLocal(_lazyStringLocal);
            _generator.LoadLocal(indexLocal);
            Type tupleType = null;
            if(type == typeof(DateTime))
            {
                tupleType = StaticCall<DateTime, DateTimeEmitter>("ToDateTime", _generator);
            }
            else if(type == typeof(DateTime?))
            {
                tupleType = StaticCall<DateTime?, DateTimeEmitter>("ToNullableDateTime", _generator);
            }
            else
            {
                throw new InvalidOperationException($"Not a DateTime Type {type}");
            }
            _generator.Duplicate();

            _generator.LoadField(tupleType.GetRuntimeField("Item2"));
            _generator.StoreLocal(indexLocal);

            _generator.LoadField(tupleType.GetRuntimeField("Item1"));
        }

        Type StaticCall<T,Emitter>(string methodName, JsonILGenerator generator)
        {
            generator.Call(typeof(Emitter).GetRuntimeMethod(methodName, new Type[]{typeof(LazyString), typeof(int)}));
            return typeof(ValueTuple<T,int>);
        }

        public static (DateTime?, int) ToNullableDateTime(LazyString lazyString, int index)
        {
            string buffer = lazyString.Buffer;
            int start = lazyString.Start;
            index += start;

            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = buffer[index];
                if(character == 'n')
                {
                    return (null, index + 4 - start);
                }
                else if(character == '\"')
                {
                    break;
                }
                index++;
            }
            index++;
            int year = 
                (buffer[index] - 48)*1000 + 
                (buffer[index + 1] - 48)*100 +
                (buffer[index + 2] - 48)*10 +
                (buffer[index + 3] - 48);
            int month =
                (buffer[index + 5] - 48)*10 +
                (buffer[index + 6] - 48);
            int day =
                (buffer[index + 8] - 48)*10 +
                (buffer[index + 9] - 48);

            if(buffer[index + 10] == '\"')
            {
                return (new DateTime(year, month, day), index + 11 - start);
            }

            int hour =
                (buffer[index + 11] - 48)*10 +
                (buffer[index + 12] - 48);

            int minute =
                (buffer[index + 14] - 48)*10 +
                (buffer[index + 15] - 48);

            int second =
                (buffer[index + 17] - 48)*10 +
                (buffer[index + 18] - 48);

            
            index += 19;
            character = buffer[index];
            if(character == '\"')
            {
                return (new DateTime(year, month, day, hour, minute, second), index + 1 - start);
            }

            double milliseonds = 0;
            if(character == '.')
            {
                index++;
                //milliseconds
                int subSeconds = 0;
                int millisecondsStart = index;
                
                while(true)
                {
                    character = buffer[index];
                    if(character >= '0' && character <= '9')
                    {
                        subSeconds = (subSeconds * 10) + (character - 48);
                        index++;
                    }
                    else
                    {
                        int millisecondsLength = index - millisecondsStart;
                        double multiplier = 1;
                        switch(millisecondsLength)
                        {
                            case 1: 
                                multiplier = 100;
                                break;
                            case 2:
                                multiplier = 10;
                                break;
                            case 3:
                                multiplier = 1;
                                break;
                            case 4:
                                multiplier = 0.1d;
                                break;
                            case 5:
                                multiplier = 0.01d;
                                break;
                            case 6:
                                multiplier = 0.001d;
                                break;
                            case 7:
                                multiplier = 0.0001d;
                                break;
                            case 8:
                                multiplier = 0.00001d;
                                break;
                        }
                        milliseonds = subSeconds*multiplier;
                        break;
                    }
                }
            }
            DateTimeKind kind = DateTimeKind.Unspecified;
            if(character == '\"')
            {

            }
            else if(character == 'Z')
            {
                kind = DateTimeKind.Utc;
                index++;
            }
            else
            {
                int offsetSign = character == '-' ? -1 : 1;
                int offsetHours = 
                    (buffer[index + 1] - 48)*10 +
                    (buffer[index + 2] - 48);
                int offsetMinutes = 
                    (buffer[index + 4] - 48)*10 +
                    (buffer[index + 5] - 48);
                var offset = new TimeSpan(offsetSign*offsetHours, offsetMinutes, 0);
                var localDateTime = new DateTimeOffset(year, month, day, hour, minute, second, offset).AddMilliseconds(milliseonds).LocalDateTime;
                return (localDateTime, index + 7 - start);
            }

            var dateTime = new DateTime(year, month, day, hour, minute, second, kind).AddMilliseconds(milliseonds);
            return (dateTime, index + 1 - start);
        }

        public static (DateTime, int) ToDateTime(LazyString lazyString, int index)
        {
            string buffer = lazyString.Buffer;
            int start = lazyString.Start;
            index += start;

            //skip any whitespace at start
            char character = ' ';
            while(true)
            {
                character = buffer[index];
                if(character == '\"')
                {
                    break;
                }
                index++;
            }
            index++;
            int year = 
                (buffer[index] - 48)*1000 + 
                (buffer[index + 1] - 48)*100 +
                (buffer[index + 2] - 48)*10 +
                (buffer[index + 3] - 48);
            int month =
                (buffer[index + 5] - 48)*10 +
                (buffer[index + 6] - 48);
            int day =
                (buffer[index + 8] - 48)*10 +
                (buffer[index + 9] - 48);

            if(buffer[index + 10] == '\"')
            {
                return (new DateTime(year, month, day), index + 11 - start);
            }

            int hour =
                (buffer[index + 11] - 48)*10 +
                (buffer[index + 12] - 48);

            int minute =
                (buffer[index + 14] - 48)*10 +
                (buffer[index + 15] - 48);

            int second =
                (buffer[index + 17] - 48)*10 +
                (buffer[index + 18] - 48);

            
            index += 19;
            character = buffer[index];
            if(character == '\"')
            {
                return (new DateTime(year, month, day, hour, minute, second), index + 1 - start);
            }

            double milliseonds = 0;
            if(character == '.')
            {
                index++;
                //milliseconds
                int subSeconds = 0;
                int millisecondsStart = index;
                
                while(true)
                {
                    character = buffer[index];
                    if(character >= '0' && character <= '9')
                    {
                        subSeconds = (subSeconds * 10) + (character - 48);
                        index++;
                    }
                    else
                    {
                        int millisecondsLength = index - millisecondsStart;
                        double multiplier = 1;
                        switch(millisecondsLength)
                        {
                            case 1: 
                                multiplier = 100;
                                break;
                            case 2:
                                multiplier = 10;
                                break;
                            case 3:
                                multiplier = 1;
                                break;
                            case 4:
                                multiplier = 0.1d;
                                break;
                            case 5:
                                multiplier = 0.01d;
                                break;
                            case 6:
                                multiplier = 0.001d;
                                break;
                            case 7:
                                multiplier = 0.0001d;
                                break;
                            case 8:
                                multiplier = 0.00001d;
                                break;
                        }
                        milliseonds = subSeconds*multiplier;
                        break;
                    }
                }
            }
            DateTimeKind kind = DateTimeKind.Unspecified;
            if(character == '\"')
            {

            }
            else if(character == 'Z')
            {
                kind = DateTimeKind.Utc;
                index++;
            }
            else
            {
                int offsetSign = character == '-' ? -1 : 1;
                int offsetHours = 
                    (buffer[index + 1] - 48)*10 +
                    (buffer[index + 2] - 48);
                int offsetMinutes = 
                    (buffer[index + 4] - 48)*10 +
                    (buffer[index + 5] - 48);
                var offset = new TimeSpan(offsetSign*offsetHours, offsetMinutes, 0);
                var localDateTime = new DateTimeOffset(year, month, day, hour, minute, second, offset).AddMilliseconds(milliseonds).LocalDateTime;
                return (localDateTime, index + 7 - start);
            }

            var dateTime = new DateTime(year, month, day, hour, minute, second, kind).AddMilliseconds(milliseonds);
            return (dateTime, index + 1 - start);
        }

        public override bool TypeSupported(Type type)
        {
            return 
                type == typeof(DateTime) ||
                type == typeof(DateTime?);
        }
    }
}