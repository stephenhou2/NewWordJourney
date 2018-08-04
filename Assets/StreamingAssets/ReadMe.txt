****************** 该文件夹内存放游戏bundle和主体数据************

- AssetBundle中存放游戏bundle
- Data中存放游戏主体数据，Data文件夹不是BuildAssetBundle出来的，更新时需要手动将文件拖入该文件夹中
  - 注意：安装app时需要将Data文件夹拷贝到本地，然后对本地（application.persistDataPath下）数据进行操作【本地路径下的文件拥有读/写权限】