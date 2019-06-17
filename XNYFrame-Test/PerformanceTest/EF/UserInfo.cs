using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;


namespace XNYFrame_Test.PerformanceTest.EF
{

    [Table("UserInfo")]
    public partial class UserInfo
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? Sex { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(20)]
        public string CardId { get; set; }

        public int? Age { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }
    }
}
