using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation.Projects;

namespace Wilmar.Service.Core.Projects
{
    /// <summary>
    /// 项目数据上下文
    /// </summary>
    public class ProjectsDataContext : DbContext
    {
        public ProjectsDataContext() :
            base("name=DesignDbContext")
        { }
        /// <summary>
        /// 项目数据集合
        /// </summary>
        public DbSet<ProjectData> ProjectDatas { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProjectData>().Ignore(a => a.LockName);
        }
    }
}
