using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dal.Interfaces;

namespace Dal.Entitites.Parent
{
    public class DbEntity : IDbEntity
    {
        [Key, Column("id")]
        public long Id { get; set; }
    }
}