using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

using Sanssoussi.Areas.Identity.Data.Config;
using Sanssoussi.Areas.Identity.Data.Config.Const;


namespace Sanssoussi.Areas.Identity.Data.Config
{
    public class RulesConfig : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData( new IdentityRole{ Id = "", Name = Rules.Client, NormalizedName = Rules.Client.ToUpper()},
                                new IdentityRole{ Id = "", Name = Rules.Admin, NormalizedName = Rules.Admin.ToUpper() } );
        }

    }
}
