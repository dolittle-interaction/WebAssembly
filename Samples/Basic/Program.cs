﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Dolittle.Execution;
using Dolittle.Serialization.Json;
using Dolittle.Runtime.Commands.Coordination;
using Dolittle.Queries.Coordination;
using Dolittle.Tenancy;
using Dolittle.Logging;
using Dolittle.Artifacts;
using Dolittle.PropertyBags;
using Dolittle.Commands;
using Dolittle.Collections;
using Dolittle.Booting;
using Basic.MyFeature;
using Dolittle.Runtime.Commands;
using Dolittle.Queries;
using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
using System.Threading;


namespace Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            var before = DateTime.Now;

            var bootResult = Bootloader.Configure(_ => _
                .WithEntryAssembly(typeof(Program).Assembly)
                .WithAssembliesSpecifiedIn(typeof(Program).Assembly)
                .SynchronousScheduling()
                .UseLogAppender(new CustomLogAppender())
            ).Start();

            var container = bootResult.Container;
            var logger = container.Get<ILogger>();
            logger.Information("We're running");

            //SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_WebAssembly());

            //SampleClass.Run().Wait();

        }
    }

#if(false)
	public class SampleClass
	{
		public static async Task Run()
		{
			using (var db = new BloggingContext())
			{
				db.Database.EnsureCreated();

				Console.WriteLine("Database created");

				db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
				var count = await db.SaveChangesAsync(CancellationToken.None);

				Console.WriteLine("{0} records saved to database", count);

				Console.WriteLine();
				Console.WriteLine("All blogs in database:");
				foreach (var blog in db.Blogs)
				{
					Console.WriteLine(" - {0}", blog.Url);
				}
			}
		}
	}

	public class Blog
	{
		public int BlogId { get; set; }
		public string Url { get; set; }

		public List<Post> Posts { get; set; }
	}

	public class Post
	{
		public int PostId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }

		public int BlogId { get; set; }
		public Blog Blog { get; set; }
	}

	public class BloggingContext : DbContext
	{
		public DbSet<Blog> Blogs { get; set; }
		public DbSet<Post> Posts { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			// Uncomment those to enable logging
			// optionsBuilder.UseLoggerFactory(LogExtensionPoint.AmbientLoggerFactory);
			// optionsBuilder.EnableSensitiveDataLogging(true);

			optionsBuilder.UseSqlite($"data source=local.db");
		}
	}    

#endif    
}