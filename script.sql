USE [master]
GO
/****** Object:  Database [SMS]    Script Date: 13-08-2017 7:32:27 PM ******/
CREATE DATABASE [SMS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SMS', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQL\MSSQL\DATA\SMS.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SMS_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQL\MSSQL\DATA\SMS_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [SMS] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SMS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SMS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SMS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SMS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SMS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SMS] SET ARITHABORT OFF 
GO
ALTER DATABASE [SMS] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SMS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SMS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SMS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SMS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SMS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SMS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SMS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SMS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SMS] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SMS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SMS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SMS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SMS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SMS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SMS] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SMS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SMS] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SMS] SET  MULTI_USER 
GO
ALTER DATABASE [SMS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SMS] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SMS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SMS] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SMS] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [SMS] SET QUERY_STORE = OFF
GO
USE [SMS]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [SMS]
GO
/****** Object:  Table [dbo].[fees_receipt]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fees_receipt](
	[fin_id] [varchar](50) NOT NULL,
	[receipt_no] [int] NOT NULL,
	[receipt_date] [date] NOT NULL,
	[acc_id] [int] NOT NULL,
	[fees_name] [varchar](50) NULL,
	[sr_number] [int] NOT NULL,
	[class_id] [int] NOT NULL,
	[section_id] [int] NOT NULL,
	[batch_id] [int] NOT NULL,
	[amount] [decimal](18, 2) NOT NULL,
	[reg_no] [int] NULL,
	[reg_date] [date] NULL,
	[dc_fine] [decimal](18, 2) NULL,
	[dc_discount] [decimal](18, 2) NULL,
	[narration] [varchar](max) NULL,
	[serial] [int] NOT NULL,
	[dt_date] [date] NOT NULL,
	[bnk_name] [varchar](50) NULL,
	[chq_no] [varchar](50) NULL,
	[chq_date] [date] NULL,
	[mode_flag] [varchar](50) NULL,
	[clear_flag] [bit] NULL,
	[chq_reject] [varchar](50) NULL,
	[nt_clear_reason] [varchar](max) NULL,
 CONSTRAINT [PK_fees_receipt] PRIMARY KEY CLUSTERED 
(
	[fin_id] ASC,
	[receipt_no] ASC,
	[receipt_date] ASC,
	[serial] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mst_acc_head]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_acc_head](
	[acc_id] [int] NOT NULL,
	[acc_name] [varchar](50) NOT NULL,
	[nature] [varchar](50) NOT NULL,
 CONSTRAINT [PK_mst_acc_head] PRIMARY KEY CLUSTERED 
(
	[acc_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mst_batch]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_batch](
	[batch_id] [int] NOT NULL,
	[class_id] [int] NOT NULL,
 CONSTRAINT [PK_mst_batch] PRIMARY KEY CLUSTERED 
(
	[batch_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mst_class]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_class](
	[class_id] [int] NOT NULL,
	[class_name] [varchar](max) NULL,
 CONSTRAINT [PK_mst_class] PRIMARY KEY CLUSTERED 
(
	[class_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mst_fees]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_fees](
	[class_id] [int] NOT NULL,
	[acc_id] [int] NOT NULL,
	[fees_amount] [decimal](18, 2) NOT NULL,
	[bl_onetime] [bit] NOT NULL,
	[bl_apr] [bit] NOT NULL,
	[bl_may] [bit] NOT NULL,
	[bl_jun] [bit] NOT NULL,
	[bl_jul] [bit] NOT NULL,
	[bl_aug] [bit] NOT NULL,
	[bl_sep] [bit] NOT NULL,
	[bl_oct] [bit] NOT NULL,
	[bl_nov] [bit] NOT NULL,
	[bl_dec] [bit] NOT NULL,
	[bl_jan] [bit] NOT NULL,
	[bl_feb] [bit] NOT NULL,
	[bl_mar] [bit] NOT NULL,
 CONSTRAINT [PK_mst_fees] PRIMARY KEY CLUSTERED 
(
	[class_id] ASC,
	[acc_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mst_fin]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_fin](
	[fin_id] [varchar](50) NOT NULL,
	[fin_start_date] [date] NOT NULL,
	[fin_end_date] [date] NOT NULL,
	[fin_close] [varchar](10) NOT NULL,
 CONSTRAINT [PK_mst_fin] PRIMARY KEY CLUSTERED 
(
	[fin_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mst_section]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_section](
	[section_id] [int] NOT NULL,
	[class_id] [int] NOT NULL,
	[section_name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_mst_section] PRIMARY KEY CLUSTERED 
(
	[section_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mst_session]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_session](
	[session] [varchar](50) NOT NULL,
	[session_start_date] [date] NOT NULL,
	[session_end_date] [date] NOT NULL,
	[session_active] [varchar](10) NOT NULL,
 CONSTRAINT [PK_mst_session] PRIMARY KEY CLUSTERED 
(
	[session] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mst_transport]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_transport](
	[pickup_id] [int] NOT NULL,
	[pickup_point] [varchar](50) NOT NULL,
	[transport_fees] [decimal](18, 2) NULL,
	[transport_number] [varchar](50) NULL,
	[bl_apr] [bit] NOT NULL,
	[bl_may] [bit] NOT NULL,
	[bl_jun] [bit] NOT NULL,
	[bl_jul] [bit] NOT NULL,
	[bl_aug] [bit] NOT NULL,
	[bl_sep] [bit] NOT NULL,
	[bl_oct] [bit] NOT NULL,
	[bl_nov] [bit] NOT NULL,
	[bl_dec] [bit] NOT NULL,
	[bl_jan] [bit] NOT NULL,
	[bl_feb] [bit] NOT NULL,
	[bl_mar] [bit] NOT NULL,
 CONSTRAINT [PK_mst_transport] PRIMARY KEY CLUSTERED 
(
	[pickup_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[out_standing]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[out_standing](
	[serial] [int] NOT NULL,
	[fin_id] [varchar](50) NOT NULL,
	[dt_date] [date] NOT NULL,
	[acc_id] [int] NOT NULL,
	[sr_number] [int] NOT NULL,
	[outstd_amount] [decimal](18, 2) NOT NULL,
	[rmt_amount] [decimal](18, 2) NULL,
	[narration] [varchar](max) NULL,
	[reg_num] [int] NULL,
	[month_name] [varchar](50) NULL,
	[month_no] [int] NULL,
	[clear_flag] [bit] NULL,
	[receipt_no] [int] NULL,
	[receipt_date] [date] NULL,
	[class_id] [int] NULL,
 CONSTRAINT [PK_out_standing] PRIMARY KEY CLUSTERED 
(
	[serial] ASC,
	[fin_id] ASC,
	[dt_date] ASC,
	[acc_id] ASC,
	[sr_number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sr_register]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sr_register](
	[sr_number] [int] NOT NULL,
	[std_first_name] [varchar](50) NOT NULL,
	[std_last_name] [varchar](50) NULL,
	[std_father_name] [varchar](50) NOT NULL,
	[std_mother_name] [varchar](50) NOT NULL,
	[std_address] [varchar](50) NOT NULL,
	[std_address1] [varchar](50) NULL,
	[std_address2] [varchar](50) NULL,
	[std_district] [varchar](50) NOT NULL,
	[std_state] [varchar](50) NOT NULL,
	[std_country] [varchar](50) NOT NULL,
	[std_pincode] [varchar](50) NOT NULL,
	[std_contact] [varchar](50) NOT NULL,
	[std_contact1] [varchar](50) NULL,
	[std_contact2] [varchar](50) NULL,
	[std_email] [varchar](50) NULL,
	[std_father_occupation] [varchar](50) NULL,
	[std_mother_occupation] [varchar](50) NULL,
	[std_blood_gp] [varchar](50) NULL,
	[std_house_income] [varchar](50) NULL,
	[std_nationality] [varchar](50) NOT NULL,
	[std_category] [varchar](50) NOT NULL,
	[std_cast] [varchar](50) NOT NULL,
	[std_dob] [date] NOT NULL,
	[std_sex] [varchar](50) NOT NULL,
	[std_last_school] [varchar](50) NULL,
	[std_admission_date] [date] NOT NULL,
	[std_section_id] [int] NOT NULL,
	[std_batch_id] [int] NOT NULL,
	[std_house] [varchar](50) NULL,
	[std_remark] [varchar](max) NULL,
	[std_active] [nchar](2) NOT NULL,
	[std_pickup_id] [int] NOT NULL,
	[std_admission_class] [varchar](50) NOT NULL,
	[adm_session] [varchar](50) NOT NULL,
	[reg_no] [varchar](50) NOT NULL,
	[reg_date] [date] NOT NULL,
 CONSTRAINT [PK_sr_register] PRIMARY KEY CLUSTERED 
(
	[sr_number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[std_registration]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[std_registration](
	[session] [varchar](50) NOT NULL,
	[reg_no] [int] NOT NULL,
	[reg_date] [date] NOT NULL,
	[std_first_name] [varchar](50) NOT NULL,
	[std_last_name] [varchar](50) NULL,
	[std_father_name] [varchar](50) NOT NULL,
	[std_mother_name] [varchar](50) NOT NULL,
	[std_address] [varchar](50) NOT NULL,
	[std_address1] [varchar](50) NULL,
	[std_address2] [varchar](50) NULL,
	[std_district] [varchar](50) NOT NULL,
	[std_state] [varchar](50) NOT NULL,
	[std_country] [varchar](50) NOT NULL,
	[std_pincode] [varchar](50) NOT NULL,
	[std_contact] [varchar](50) NOT NULL,
	[std_contact1] [varchar](50) NULL,
	[std_contact2] [varchar](50) NULL,
	[std_email] [varchar](50) NULL,
	[std_class_id] [int] NOT NULL,
 CONSTRAINT [PK_std_registration] PRIMARY KEY CLUSTERED 
(
	[session] ASC,
	[reg_no] ASC,
	[reg_date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[transaction]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[transaction](
	[fin_id] [varchar](50) NOT NULL,
	[trans_no] [int] NOT NULL,
	[trans_date] [date] NOT NULL,
	[outstand_amt] [decimal](18, 2) NULL,
	[rmt_amt] [decimal](18, 2) NULL,
	[fine] [decimal](18, 2) NULL,
	[discount] [decimal](18, 2) NULL,
	[sr_no] [int] NOT NULL,
	[chq_reject] [varchar](50) NULL,
	[bnk_name] [varchar](50) NULL,
	[chq_date] [date] NULL,
	[chq_no] [varchar](50) NULL,
	[acc_id] [int] NOT NULL,
	[serial] [int] NULL,
	[serial_date] [date] NULL,
	[receipt_no] [int] NULL,
	[receipt_date] [date] NULL,
	[month_name] [varchar](50) NULL,
	[mode] [varchar](50) NULL,
	[class_id] [int] NULL,
 CONSTRAINT [PK_transaction] PRIMARY KEY CLUSTERED 
(
	[fin_id] ASC,
	[trans_no] ASC,
	[trans_date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[MonthlyFees]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO













CREATE PROCEDURE [dbo].[MonthlyFees] @sr_num int
as
begin 



declare @bl_apr bit;
declare @bl_may bit;
declare @bl_jun bit;
declare @bl_jul bit;
declare @bl_aug bit;
declare @bl_sep bit;
declare @bl_oct bit;
declare @bl_nov bit;
declare @bl_dec bit;
declare @bl_jan bit;
declare @bl_feb bit;
declare @bl_mar bit;
declare @acc_id int;
declare @amount decimal;
declare @finid varchar(max);
declare @class_id int;
declare @end int;
declare @year varchar(10);
declare @year1 varchar(10);

set @class_id =  (select b.class_id from [sms].[dbo].sr_register a,[sms].[dbo].mst_section b
							where a.std_section_id = b.section_id
							and a.std_active = 'Y'
								and a.sr_number = @sr_num);
 



set @finid = (select fin_id from [SMS].[dbo].mst_fin where fin_close = 'N');


declare fees cursor local for SELECT [acc_id]
      ,[fees_amount]
      ,[bl_apr]
      ,[bl_may]
      ,[bl_jun]
      ,[bl_jul]
      ,[bl_aug]
      ,[bl_sep]
      ,[bl_oct]
      ,[bl_nov]
      ,[bl_dec]
      ,[bl_jan]
      ,[bl_feb]
      ,[bl_mar]
  FROM [SMS].[dbo].[mst_fees]
  where class_id =  @class_id;
  
  open fees;

  fetch fees into @acc_id, @amount,@bl_apr,@bl_may,@bl_jun,@bl_jul,@bl_aug,@bl_sep,@bl_oct,@bl_nov,@bl_dec,@bl_jan,@bl_feb,@bl_mar;

  if  (DATEPART(M,getdate()) between 4 and 12)
  begin
	
		set @end = DATEPART(M,getdate());
		set @year = convert(varchar,DATEPART(YYYY,GETDATE()));
	 end
	else
	begin
		set @end = DATEPART(M,getdate()) + 12;
		set @year = convert(varchar,DATEPART(YYYY,GETDATE())-1);
		set @year1 = convert(varchar,DATEPART(YYYY,GETDATE()));
	end;


  WHILE @@FETCH_STATUS = 0
	begin
	 
	
			if((@bl_apr=1) AND (4 <= @end))
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 4 , -1 ) ) + ' ' + @year
           ,DATEPART(M,getdate()))
			end;

			if((@bl_may=1) AND (5 <= @end))
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 5 , -1 ) ) + ' ' + @year
           ,DATEPART(M,getdate()))
			end;

			if((@bl_jun=1) AND (6 <= @end))
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 6 , -1 ) ) + ' ' + @year
           ,DATEPART(M,getdate()))
			end;

			if((@bl_jul=1) AND (7 <= @end))
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 7 , -1 ) ) + ' ' + @year
           ,DATEPART(M,getdate()))
			end;

			if((@bl_aug=1) AND (8 <= @end))
			begin 
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 8 , -1 ) ) + ' ' + @year
           ,DATEPART(M,getdate()))
			end;

			if((@bl_sep=1) AND (9 <= @end))
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 9 , -1 ) ) + ' ' + @year
           ,DATEPART(M,getdate()))
			end;

			if((@bl_oct=1) AND (10 <= @end))
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 10 , -1 ) ) + ' ' + @year
           ,DATEPART(M,getdate()))
			end;

			if((@bl_nov=1) AND (11 <= @end))
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 11 , -1 ) ) + ' ' + @year
           ,DATEPART(M,getdate()))
			end;

			if((@bl_dec=1) AND (12 <= @end))
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 12 , -1 ) ) + ' ' + @year
           ,DATEPART(M,getdate()))
			end;

			if((@bl_jan=1) AND (13 <= @end))
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 1 , -1 ) ) + ' ' + @year1
           ,DATEPART(M,getdate()))
			end;

			if((@bl_feb=1) AND (14 <= @end))
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 2 , -1 ) ) + ' ' +@year1
           ,DATEPART(M,getdate()))
			end;

			if((@bl_mar=1) AND (15 <= @end))
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DateName( month , DateAdd( month , 3 , -1 ) ) + ' ' +@year1
           ,DATEPART(M,getdate()))
			end;


			
		fetch fees into @acc_id, @amount,@bl_apr,@bl_may,@bl_jun,@bl_jul,@bl_aug,@bl_sep,@bl_oct,@bl_nov,@bl_dec,@bl_jan,@bl_feb,@bl_mar;
	end;

	close fees;
	deallocate fees;
	



end;
GO
/****** Object:  StoredProcedure [dbo].[MonthlyFeesUpdate]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE PROCEDURE [dbo].[MonthlyFeesUpdate]
as
begin 

declare @sr_num int;
declare @class_id int;
declare @bl_apr bit;
declare @bl_may bit;
declare @bl_jun bit;
declare @bl_jul bit;
declare @bl_aug bit;
declare @bl_sep bit;
declare @bl_oct bit;
declare @bl_nov bit;
declare @bl_dec bit;
declare @bl_jan bit;
declare @bl_feb bit;
declare @bl_mar bit;
declare @acc_id int;
declare @amount decimal;
declare @cnt int;
declare @finid varchar(max);
DECLARE student CURSOR local FOR select a.sr_number,b.class_id from [sms].[dbo].sr_register a,[sms].[dbo].mst_batch b
							where a.std_batch_id = b.batch_id
							and a.std_active = 'Y';
 

open student;

fetch student into @sr_num, @class_id;

set @finid = (select fin_id from [SMS].[dbo].mst_fin where fin_close = 'N');

WHILE @@FETCH_STATUS = 0
begin

declare fees cursor local for SELECT [acc_id]
      ,[fees_amount]
      ,[bl_apr]
      ,[bl_may]
      ,[bl_jun]
      ,[bl_jul]
      ,[bl_aug]
      ,[bl_sep]
      ,[bl_oct]
      ,[bl_nov]
      ,[bl_dec]
      ,[bl_jan]
      ,[bl_feb]
      ,[bl_mar]
  FROM [SMS].[dbo].[mst_fees]
  where class_id =  @class_id;
  
  open fees;

  fetch fees into @acc_id, @amount,@bl_apr,@bl_may,@bl_jun,@bl_jul,@bl_aug,@bl_sep,@bl_oct,@bl_nov,@bl_dec,@bl_jan,@bl_feb,@bl_mar;



  WHILE @@FETCH_STATUS = 0
	begin
	set @cnt = 1;
		while @cnt <= 12
		begin
			if((@bl_apr=1) AND (@cnt = DATEPART(M,getdate())))  AND (@cnt = 4)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_may=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 5)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_jun=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 6)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_jul=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 7)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_aug=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 8)
			begin 
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_sep=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 9)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_oct=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 10)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_nov=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 11)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_dec=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 12)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_jan=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 1)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_feb=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 2)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_mar=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 3)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;


			set @cnt = @cnt + 1;
		end;
		fetch fees into @acc_id, @amount,@bl_apr,@bl_may,@bl_jun,@bl_jul,@bl_aug,@bl_sep,@bl_oct,@bl_nov,@bl_dec,@bl_jan,@bl_feb,@bl_mar;
	end;
	
	close fees;
	deallocate  fees;
	fetch student into @sr_num, @class_id;
	end;
	close student;
	deallocate  student;
end;

GO
/****** Object:  StoredProcedure [dbo].[MonthlyTransportUpdate]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








CREATE PROCEDURE [dbo].[MonthlyTransportUpdate]
as
begin 

declare @sr_num int;
declare @pick_up_id int;
declare @bl_apr bit;
declare @bl_may bit;
declare @bl_jun bit;
declare @bl_jul bit;
declare @bl_aug bit;
declare @bl_sep bit;
declare @bl_oct bit;
declare @bl_nov bit;
declare @bl_dec bit;
declare @bl_jan bit;
declare @bl_feb bit;
declare @bl_mar bit;
declare @acc_id int;
declare @amount decimal;
declare @cnt int;
declare @finid varchar(max);
DECLARE student CURSOR local FOR select a.sr_number,a.std_pickup_id from [sms].[dbo].sr_register a
							where a.std_active = 'Y';
 

open student;

fetch student into @sr_num, @pick_up_id;

set @finid = (select fin_id from [SMS].[dbo].mst_fin where fin_close = 'N');

WHILE @@FETCH_STATUS = 0
begin

declare fees cursor local for SELECT  6 acc_id, [transport_fees]
      ,[bl_apr]
      ,[bl_may]
      ,[bl_jun]
      ,[bl_jul]
      ,[bl_aug]
      ,[bl_sep]
      ,[bl_oct]
      ,[bl_nov]
      ,[bl_dec]
      ,[bl_jan]
      ,[bl_feb]
      ,[bl_mar]
  FROM [SMS].[dbo].[mst_transport]
  where pickup_id = @pick_up_id
		and [transport_fees] > 0;
  
  
  open fees;

  fetch fees into @acc_id,@amount,@bl_apr,@bl_may,@bl_jun,@bl_jul,@bl_aug,@bl_sep,@bl_oct,@bl_nov,@bl_dec,@bl_jan,@bl_feb,@bl_mar;



  WHILE @@FETCH_STATUS = 0
	begin
	set @cnt = 1;
		while @cnt <= 12
		begin
			if((@bl_apr=1) AND (@cnt = DATEPART(M,getdate())))  AND (@cnt = 4)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_may=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 5)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_jun=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 6)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_jul=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 7)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_aug=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 8)
			begin 
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_sep=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 9)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_oct=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 10)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_nov=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 11)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_dec=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 12)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_jan=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 1)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_feb=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 2)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;

			if((@bl_mar=1) AND (@cnt = DATEPART(M,getdate()))) AND (@cnt = 3)
			begin
				INSERT INTO [SMS].[dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[month_name]
           ,[month_no])
			VALUES
           ((select max(serial)+1 from [SMS].[dbo].out_standing)
           ,@finid
           ,convert(date,getdate())
           ,@acc_id
           ,@sr_num
           ,@amount
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))
			end;


			set @cnt = @cnt + 1;
		end;
		fetch fees into @acc_id, @amount,@bl_apr,@bl_may,@bl_jun,@bl_jul,@bl_aug,@bl_sep,@bl_oct,@bl_nov,@bl_dec,@bl_jan,@bl_feb,@bl_mar;
	end;
	
	close fees;
	
	deallocate  fees;
	fetch student into @sr_num, @pick_up_id;
	end;
	
	close student;
	deallocate  student;
end;

GO
/****** Object:  Trigger [dbo].[transaction_insert_on_receipt]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE TRIGGER [dbo].[transaction_insert_on_receipt]
       ON [dbo].[fees_receipt]
AFTER INSERT
AS
BEGIN
			DECLARE @max INT
			DECLARE @receipt_no INT
			DECLARE @fin_id varchar(50)
			DECLARE @receipt_date date
			DECLARE @sr_num int
			DECLARE @amt decimal(18,2)
			DECLARE @fine decimal(18,2)
			DECLARE @discount decimal(18,2)
			DECLARE @acc_id int
			DECLARE @bnk_name varchar(50)
			DECLARE @chq_no varchar(50)
			DECLARE @chq_date date
			DECLARE @class_id int
			DECLARE @chq_reject varchar(50)
			DECLARE @mode varchar(50)
			DECLARE @serial int

	set @max =	(SELECT isnull(MAX(trans_no),0)+1 FROM [dbo].[transaction]) ; 

	 SELECT @receipt_no = INSERTED.receipt_no,
	 @receipt_date = INSERTED.receipt_date, 
	 @fin_id = INSERTED.fin_id,   
	 @sr_num = INSERTED.sr_number, 
	 @amt = INSERTED.amount,
	 @acc_id = INSERTED.acc_id,
	 @fine = inserted.dc_fine,
	 @discount = inserted.dc_discount,
	 @bnk_name = inserted.bnk_name,
	 @chq_no=inserted.chq_no,
	 @chq_date = inserted.chq_date,
	 @class_id = inserted.class_id,
	 @chq_reject = inserted.chq_reject,
	  @mode = inserted.mode_flag,
	  @serial = inserted.serial
       FROM INSERTED

	   INSERT INTO [dbo].[transaction]
           ([fin_id]
           ,[trans_no]
           ,[trans_date]
           ,[rmt_amt]
           ,[fine]
           ,[discount]
           ,[sr_no]
           ,[chq_reject]
           ,[bnk_name]
           ,[chq_date]
           ,[chq_no]
           ,[acc_id]
           ,[receipt_no]
           ,[receipt_date]
           ,[mode]
           ,[class_id]
		   ,[serial])
     VALUES
           (@fin_id 
           ,@max
           ,CONVERT(date, getdate())
           ,@amt
           ,@fine
           ,@discount
           ,@sr_num
           ,@chq_reject 
           ,@bnk_name
           ,@chq_date
           ,@chq_no
           ,@acc_id
           ,@receipt_no
           ,@receipt_date
           ,@mode
           ,@class_id
		   ,@serial)
      
	
END
GO
ALTER TABLE [dbo].[fees_receipt] ENABLE TRIGGER [transaction_insert_on_receipt]
GO
/****** Object:  Trigger [dbo].[section_insert]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[section_insert]
       ON [dbo].[mst_class]
AFTER INSERT
AS
BEGIN
			DECLARE @max INT
			DECLARE @class_id INT


	set @max =	(SELECT max(section_id)+1 FROM mst_section) ; 

	 SELECT @class_id = INSERTED.class_id       
       FROM INSERTED

       INSERT INTO mst_section
       VALUES(@max ,@class_id , 'A')
END
GO
ALTER TABLE [dbo].[mst_class] ENABLE TRIGGER [section_insert]
GO
/****** Object:  Trigger [dbo].[transaction_insert]    Script Date: 13-08-2017 7:32:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE TRIGGER [dbo].[transaction_insert]
       ON [dbo].[out_standing]
AFTER INSERT
AS
BEGIN
			DECLARE @max INT
			DECLARE @serial INT
			DECLARE @fin_id varchar(50)
			DECLARE @serial_date date
			DECLARE @sr_num int
			DECLARE @amt decimal(18,2)
			DECLARE @month_name varchar(50)
			DECLARE @acc_id int
			DECLARE @class_id int

	set @max =	(SELECT isnull(MAX(trans_no),0)+1 FROM [dbo].[transaction]) ; 

	 SELECT @serial = INSERTED.serial,@serial_date = INSERTED.dt_date, @fin_id = INSERTED.fin_id,   @sr_num = INSERTED.sr_number, @amt = INSERTED.outstd_amount,@month_name = INSERTED.month_name,@acc_id = INSERTED.acc_id, @class_id = INSERTED.class_id
       FROM INSERTED

      
	   INSERT INTO [dbo].[transaction]
           ([fin_id]
           ,[trans_no]
           ,[trans_date]
           ,[outstand_amt]
           ,[sr_no]
           ,[acc_id]
           ,[serial]
           ,[serial_date]
           ,[month_name]
		   ,[class_id])
     VALUES
           (@fin_id
           ,@max
           ,CONVERT(date, getdate())
           , @amt
           ,@sr_num
           ,@acc_id
           ,@serial
           ,@serial_date
           ,@month_name
		   ,@class_id)
END
GO
ALTER TABLE [dbo].[out_standing] ENABLE TRIGGER [transaction_insert]
GO
USE [master]
GO
ALTER DATABASE [SMS] SET  READ_WRITE 
GO
