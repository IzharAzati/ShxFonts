CREATE TABLE [dbo].[ShxFontDef] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [ShxFontFileId] INT            NOT NULL,
    [ShapeNumber]   INT            CONSTRAINT [DF_ShxFontDef_ShapeNumber] DEFAULT ((0)) NOT NULL,
    [ShapeHex]      AS             (format([ShapeNumber],'X4')),
    [DefBytes]      SMALLINT       CONSTRAINT [DF_ShxFontDef_DefBytes] DEFAULT ((0)) NOT NULL,
    [ShapeName]     NVARCHAR (130) NULL,
    [ShapeLine]     SMALLINT       CONSTRAINT [DF_ShxFontDef_ShapeLine] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ShxFontDef] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ShxFontDef_ShxFontFiles] FOREIGN KEY ([ShxFontFileId]) REFERENCES [dbo].[ShxFontFiles] ([Id]) ON DELETE CASCADE
);

