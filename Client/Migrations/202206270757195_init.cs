﻿namespace Client.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CoreSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaxLogCount = c.Int(nullable: false),
                        APIUrl = c.String(maxLength: 500),
                        LoginTitle = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        ParentId = c.Int(nullable: false),
                        Remark = c.String(maxLength: 200),
                        Index = c.Int(nullable: false),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelRemark = c.String(maxLength: 200),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DepartmentPositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        DepartmentId = c.Int(nullable: false),
                        Remark = c.String(maxLength: 200),
                        Index = c.Int(nullable: false),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelRemark = c.String(maxLength: 200),
                        MaxUserCount = c.Int(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Emails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FromId = c.Int(nullable: false),
                        EmailType = c.Int(nullable: false),
                        Content = c.String(),
                        Title = c.String(),
                        SendTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmailSendToes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmailId = c.Int(nullable: false),
                        EmailTitle = c.String(),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        UserReadTime = c.String(),
                        SendTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogType = c.Int(nullable: false),
                        LogStr = c.String(),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ModulePages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PluginsId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        PageName = c.String(maxLength: 50),
                        PagePath = c.String(maxLength: 500),
                        Icon = c.String(maxLength: 20),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Plugins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        DLLName = c.String(maxLength: 100),
                        DLLs = c.String(maxLength: 1000),
                        LogoImage = c.String(maxLength: 200),
                        WebDownload = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                        ConnectionName = c.String(maxLength: 100),
                        ConnectionString = c.String(maxLength: 1000),
                        DependentIds = c.String(maxLength: 500),
                        IsEnable = c.Boolean(nullable: false),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PluginsModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PluginsId = c.Int(nullable: false),
                        ModuleName = c.String(maxLength: 50),
                        Icon = c.String(maxLength: 20),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelUserName = c.String(maxLength: 50),
                        DelTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RolePlugins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        Pages = c.String(maxLength: 200),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Pwd = c.String(maxLength: 100),
                        RoleId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        DepartmentPositionId = c.Int(nullable: false),
                        PositionEndTime = c.DateTime(nullable: false),
                        NewPositionId = c.Int(nullable: false),
                        NewPositionStartTime = c.DateTime(nullable: false),
                        PositionType = c.Int(nullable: false),
                        RealName = c.String(),
                        IdCard = c.String(),
                        CanLogin = c.Boolean(nullable: false),
                        IsDel = c.Boolean(nullable: false),
                        DelUser = c.Int(nullable: false),
                        DelTime = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserPlugins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        IncreasePages = c.String(),
                        DecrementPages = c.String(),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserPlugins");
            DropTable("dbo.Users");
            DropTable("dbo.RolePlugins");
            DropTable("dbo.Roles");
            DropTable("dbo.PluginsModules");
            DropTable("dbo.Plugins");
            DropTable("dbo.ModulePages");
            DropTable("dbo.Logs");
            DropTable("dbo.EmailSendToes");
            DropTable("dbo.Emails");
            DropTable("dbo.DepartmentPositions");
            DropTable("dbo.Departments");
            DropTable("dbo.CoreSettings");
        }
    }
}
