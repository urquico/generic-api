/****** Object:  Table [fmis].[roles]    Script Date: 6/6/2025 2:13:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [fmis].[roles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[role_name] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[role_status] [bit] NULL,
	[created_at] [datetime2](7) NULL,
	[created_by] [int] NULL,
	[updated_at] [datetime2](7) NULL,
	[updated_by] [int] NULL,
	[deleted_at] [datetime2](7) NULL,
	[deleted_by] [int] NULL,
 CONSTRAINT [PK__roles__3213E83FF091C0D4] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [fmis].[roles] ADD  DEFAULT (getdate()) FOR [created_at]
GO

