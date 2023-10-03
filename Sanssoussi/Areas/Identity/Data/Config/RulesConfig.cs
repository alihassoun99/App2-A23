using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

using Sanssoussi.Areas.Identity.Data.Config;


namespace Sanssoussi.Areas.Identity.Data.Config
{
    public class RulesConfig : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData( new IdentityRole{ Id = "", Name = "", NormalizedName = "" },
                                new IdentityRole{ Id = "", Name = "", NormalizedName = "" } );
        }

    }
}
