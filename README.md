# NodeGraph
Node graph control in WPF.  
You can customize a node visual by style.  
So you dont need to implement code behavior in views.  
You can keep MVVM design because pure binding V-VM only on xaml.

# Package requirements
* [Livet v4.0.2](https://github.com/runceel/Livet/releases/tag/v4.0.2)
* [XamlBehaviorsWpf v1.1.19](https://github.com/microsoft/XamlBehaviorsWpf/releases/tag/v1.1.19)

# Environment
* .NET Framework 4.7.2 or .NET6
* VisualStudio 2019/2022

# note
<code>NodeGraph.csproj</code> is using .NET Framework.  
<code>NodeGraph.NET6.csproj</code> is using .NET6.

Also <code>NodeGraph.PreviewTest.csproj</code> and <code>NodeGraphPreviewTest.NET6.csproj</code> same above rule.

# Demo 
Basic operations.  
![demo](https://raw.github.com/wiki/Jinten/NodeGraph/images/NodeGraph_Introduction.gif)  
　  
Node Grouping.  
![GroupNode](https://user-images.githubusercontent.com/9315925/85937980-d6728c00-b943-11ea-9339-9287247ca9d9.gif)
![GroupNode_Resize_Comment](https://user-images.githubusercontent.com/9315925/85938001-1a659100-b944-11ea-976c-821046211cd2.gif)  

Link Selecting and animations.
![LinkSelection](https://user-images.githubusercontent.com/9315925/165558859-ef37c593-1bc8-4f46-8d02-a4d0a01b1062.gif)

You can check behaviors in samples.
![image](https://user-images.githubusercontent.com/9315925/163401928-21420a7c-9ade-42a9-84c1-630a43463399.png)

You can change connector color, node header color, node content color and all text color in style.
![image](https://user-images.githubusercontent.com/9315925/164983471-8196ff32-96b1-47b4-97af-ef518ee3f39c.png)
　  
Used MaterialEditor in my Engine.  
![CamouflageShader](https://user-images.githubusercontent.com/9315925/85938058-7f20eb80-b944-11ea-9c21-7296a0325f8f.gif)  


# Roadmap
* optimize for rendering.
　  
# License
*  [MIT](https://github.com/Jinten/NodeGraph/blob/master/LICENSE)
