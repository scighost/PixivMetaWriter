# PixivMetaWriter

A tool for writing metadata from pixiv information especially tags.

**The project has been merged into [MyToolkit](https://github.com/scighost/MyToolkit), and this project is no longer updated.**

[简体中文](README.md) | English

### Feature

- Rename, like "[Anmi] 鵜飼い [44873217_p0].jpg"
- Write metadata
  - Title
  - Author
  - Date
  - Keywords (tags)
  - Comment
- Save as JPEG

### Usage

Double click "PixivMetaWriter.exe", then input directory path.

### Attention

- Original file will be deleted.
- The file must be named in the format "44873217_p0.jpg".
- Only support jpg and png.
- Png file will save as JPEG, but will copy to "\png".
- Converting-failure files will be copied to "\failure".

### Change Log

- **v0.1.2**  Support repeat failure files after finish.
- **v0.1.1**	Add parallel operation.
- **v0.1.0**	First commit.