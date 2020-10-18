using HTTPBackend;
using Effort;
using System.Net;
using Effort.Provider;
using System;
using System.Text;

namespace DBLayer
{
    public class DBMiddleware : BaseMiddleware
    {
#if (IN_MEMORY_DB)
        private static readonly EffortConnection connection = DbConnectionFactory.CreateTransient();

        static DBMiddleware()
        {
            using (Context = new CodeRunnerEntities(connection))
            {
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@admin.admin",
                    PasswordHash = Encoding.ASCII.GetBytes(BCrypt.Net.BCrypt.HashPassword("admin1", 12)),
                    Username = "admin"
                };

                Context.Users.Add(adminUser);
                Context.SaveChanges();
            }
        }
#endif

        public static CodeRunnerEntities Context { get; private set; }

        public override void ResolveRequest(HttpListenerContext context)
        {
#if (IN_MEMORY_DB)
            using (Context = new CodeRunnerEntities(connection))
            {
                Next.ResolveRequest(context);
            }
#else
            using (Context = new CodeRunnerEntities())
            {
                Next.ResolveRequest(context);
            }
#endif
        }
    }
}
