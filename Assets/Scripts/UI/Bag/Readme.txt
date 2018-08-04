********************* 该文件夹主要存放不同canvas中一些小游戏题上挂载的UI脚本*****************

- Common文件夹中存放的是一些被用在多个canvas中的UI脚本
- 其余文件夹中存放的是xxcanvas中的独有UI脚本
- DragAndDrop中存放的是拖拽功能脚本，对应不同的需要进行拖拽的游戏体，基本上每个游戏题都会有drag和drop，分别对应拖拽逻辑和释放逻辑
- SetCavasBounds为unity官方适配iphoneX的适配逻辑脚本，其实不使用该脚本，修改锚点和轴心点应该也能做到适配
- VerticalScroll中存放垂直方向TableView的UI逻辑，包含缓存池和复用的通用逻辑。该脚本已不使用，停止更新