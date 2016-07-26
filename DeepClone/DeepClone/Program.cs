using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneExtensions;
using ServiceStack.Text;
using VIC.CloneExtension;

namespace DeepClone
{
    public class Classes 
    {
        public int ID { get; set; }
    }

    public class Student
    {
        public string Name;
        public int Age { get; set; }
        public Classes Classes { get; set; }
        public Classes Classes1 { get; set; }
        public Classes[] Classes2 { get; set; }
        public List<Classes> Classes3 { get; set; }
        //public IEnumerable<Classes> Classes4 { get; set; }
        //public IEnumerable Classes5 { get; set; }
    }

    public static class JsonHelper
    {
        public static T JsonTo<T>(this string json)
        {
            return string.IsNullOrWhiteSpace(json) ? default(T)
                : JsonSerializer.DeserializeFromString<T>(json);
        }

        public static string ToJson<T>(this T obj)
        {
            return obj == null ? string.Empty
                : JsonSerializer.SerializeToString<T>(obj);
        }

        public static T2 Cast<T1, T2>(this T1 t)
        {
            return JsonTo<T2>(ToJson(t));
        }
    }

    class Program
    {
        

        static void Main(string[] args)
        {
            new System.Collections.Generic.Dictionary<string, string>();
            var s = new Student()
            {
                Name = "66",
                Age = 100,
                Classes = new Classes() { ID = 55},
                Classes2 = new Classes[1] { new Classes() { ID = 77 } }
                //, Classes4 = new List<Classes>() { new Classes() { ID = 664 } }
            };
            var func = s.DeepClone();
            Mapper.Initialize(cfg => cfg.CreateMap<Student, Student>());
            var dest = Mapper.Map<Student, Student>(s);
            dest = JsonHelper.Cast<Student, Student>(s);
            dest = s.GetClone();
            var a = Stopwatch.StartNew();
            var totalCount = 1000000;
            for (int i = 0; i < totalCount; i++)
            {
                var s3 = s.DeepClone();
            }
            a.Stop();
            var aa = a.ElapsedMilliseconds;
            Console.WriteLine("Expression : " + aa.ToString());

            a = Stopwatch.StartNew();
            for (int i = 0; i < totalCount; i++)
            {
                var s3 = s.GetClone();
            }
            a.Stop();
            var cc = a.ElapsedMilliseconds;
            Console.WriteLine("CloneExtensions : " + cc.ToString());

            a = Stopwatch.StartNew();
            for (int i = 0; i < totalCount; i++)
            {
                var s3 = JsonHelper.Cast<Student, Student>(s);
            }
            a.Stop();
            var dd = a.ElapsedMilliseconds;
            Console.WriteLine("ServiceStack json : " + dd.ToString());

            a = Stopwatch.StartNew();
            for (int i = 0; i < totalCount; i++)
            {
                var s3 = Mapper.Map<Student, Student>(s);
            }
            a.Stop();
            var bb = a.ElapsedMilliseconds;
            Console.WriteLine("AutoMapper : " + bb.ToString());

            Console.ReadLine();
        }
    }
}
