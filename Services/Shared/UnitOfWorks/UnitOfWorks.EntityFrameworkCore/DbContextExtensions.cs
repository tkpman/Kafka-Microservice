using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace UnitOfWorks.EntityFrameworkCore
{
    public static class DbContextExtensions
    {

        public static DbContext SeedDatabaseWithJson<TEntity>(this DbContext dbContext, string fileName)
            where TEntity : class
        {
            // We are getting the path from Assembly because of bug.
            // https://github.com/dotnet/project-system/issues/589
            var assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var fullPath = Path.Combine(assemblyPath, fileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException();

            List<TEntity> entities = new List<TEntity>();

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(fullPath))
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new PrivateSetterContractResolver()
                };

                JsonSerializer serializer = JsonSerializer.Create(settings);

                // Readd the list of entities in the json file.
                entities = (List<TEntity>)serializer.Deserialize(file, typeof(List<TEntity>));
            }

            if (entities != null)
            {
                // Add all the loaded entites to the db context.
                dbContext.Set<TEntity>().AddRange(entities);

                // Save all the entities in the context.
                dbContext.SaveChanges();
            }

            return dbContext;
        }
    }


        public class PrivateSetterContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var jProperty = base.CreateProperty(member, memberSerialization);
                if (jProperty.Writable)
                    return jProperty;

                jProperty.Writable = member.IsPropertyWithSetter();

                return jProperty;
            }
        }

        public class PrivateSetterCamelCasePropertyNamesContractResolver : CamelCasePropertyNamesContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var jProperty = base.CreateProperty(member, memberSerialization);
                if (jProperty.Writable)
                    return jProperty;

                jProperty.Writable = member.IsPropertyWithSetter();

                return jProperty;
            }
        }

        internal static class MemberInfoExtensions
        {
            internal static bool IsPropertyWithSetter(this MemberInfo member)
            {
                var property = member as PropertyInfo;

                return property?.GetSetMethod(true) != null;
            }
        }
    
}
