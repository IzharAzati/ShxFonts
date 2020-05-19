CREATE TABLE [dbo].[ShxFontFiles] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [LoadDirectoryId] INT            NOT NULL,
    [ShxFileName]     NVARCHAR (260) NOT NULL,
    [ShxFileSize]     INT            NOT NULL,
    [ShxFileDate]     DATETIME2 (7)  NOT NULL,
    [Crc32]           CHAR (10)      NOT NULL,
    [ShxType]         NVARCHAR (50)  NULL,
    [FontName]        NVARCHAR (130) NULL,
    [HeaderRemarks]   NVARCHAR (MAX) NULL,
    [HeaderBigFont]   NVARCHAR (130) NULL,
    [HeaderLine1]     NVARCHAR (130) NULL,
    [HeaderLine2]     NVARCHAR (130) NULL,
    [Above]           SMALLINT       NULL,
    [Below]           SMALLINT       NULL,
    [Modes]           TINYINT        NULL,
    [Encoding]        TINYINT        NULL,
    [Type]            TINYINT        NULL,
    [IsKeyboard]      BIT            CONSTRAINT [DF_ShxFontFiles_IsKeyboard] DEFAULT ((0)) NOT NULL,
    [IsMirror]        BIT            CONSTRAINT [DF_ShxFontFiles_IsMirror] DEFAULT ((0)) NOT NULL,
    [IsBackward]      BIT            CONSTRAINT [DF_ShxFontFiles_IsBackward] DEFAULT ((0)) NOT NULL,
    [Source]          NVARCHAR (250) NULL,
    [Remarks]         NVARCHAR (250) NULL,
    CONSTRAINT [PK_ShxFontFiles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK__ShxFontFi__LoadD__534D60F1] FOREIGN KEY ([LoadDirectoryId]) REFERENCES [dbo].[LoadDirectory] ([Id]) ON DELETE CASCADE
);




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The above value specifies the number of vector lengths above the baseline that the uppercase letters extend, and below indicates how far the lowercase letters descend below the baseline. The baseline is similar in concept to the lines on writing paper. These values define the basic character size and are used as scale factors for the height specified for the text object.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ShxFontFiles', @level2type = N'COLUMN', @level2name = N'Above';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The above value specifies the number of vector lengths above the baseline that the uppercase letters extend, and below indicates how far the lowercase letters descend below the baseline. The baseline is similar in concept to the lines on writing paper. These values define the basic character size and are used as scale factors for the height specified for the text object.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ShxFontFiles', @level2type = N'COLUMN', @level2name = N'Below';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'The modes byte should be 0 for a horizontally oriented font and 2 for a dual-orientation (horizontal or vertical) font. The special 00E (14) command code is honored only when modes is set to 2.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ShxFontFiles', @level2type = N'COLUMN', @level2name = N'Modes';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Font encoding. Uses one of the following integer values.
0 Unicode
1 Packed multibyte 1
2 Shape file', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ShxFontFiles', @level2type = N'COLUMN', @level2name = N'Encoding';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Font embedding information. Specifies whether the font is licensed. Licensed fonts must not be modified or exchanged. Bitcoded values can be added.
0 Font can be embedded
1 Font cannot be embedded
2 Embedding is read-only', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ShxFontFiles', @level2type = N'COLUMN', @level2name = N'Type';

