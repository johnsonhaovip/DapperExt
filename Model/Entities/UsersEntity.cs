using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;

namespace Model.Entities
{
    /// <summary>
    /// 实体对象
    /// </summary>
    [Serializable]
    public class UsersEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 登录名称
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 状态   1：启用  0禁用
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        //public string Remark1 { get; set; } 

    }

    /// <summary>
    /// Users：实体对象映射关系
    /// </summary>
    [Serializable]
    public class UsersEntityORMMapper : ClassMapper<UsersEntity>
    {
        public UsersEntityORMMapper()
        {
            base.Table("Users");
            //Map(f => f.Remark1).Ignore();//设置忽略 
            //Map(f => f.Name).Key(KeyType.Identity);//设置主键  (如果主键名称不包含字母“ID”，请设置)      
            AutoMap();
        }
    }
}
