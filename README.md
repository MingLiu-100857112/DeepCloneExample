# Deep Clone Example

总结 Deep Clone 一般有如下几种实现方式：

1. 纯手工每个类实现赋值           （ps: 不做介绍，一般都不想这么玩）
2. 序列化和反序列化
3. 纯反射
4. emit 或 Expression Tree

下面是本人举了几个样本：

* Expression Tree

    Expression Tree 和 emit 性能按理论来说应该差距不太大，所以这里只举 Expression Tree

    [Expression](https://github.com/fs7744/DeepCloneExample/blob/master/DeepClone/DeepClone/DeepCloneHelper.cs) 这个是举例用Expression Tree实现的“伪通用”Deep Clone方法
    比如字典，strut，接口等等都没有考虑，只是一个Demo，可以作为大家了解如何实现的一个例子，因为支持考虑少，所以性能最高

    [CloneExtensions](https://github.com/MarcinJuraszek/CloneExtensions) 一个也是用Expression Tree实现的Deep Clone库，支持多了很多，但是也有些限制，详情查阅 https://github.com/MarcinJuraszek/CloneExtensions 

* 序列化和反序列化

    序列化和反序列化 有很多序列化协议，比如json，xml，Thrift，Protobuf，Avro 等等， https://github.com/eishay/jvm-serializers/wiki 这个是一些比较结果，所以其实用序列化实现Deep Clone 完全依靠对应协议和实现的性能

    [ServiceStack json] 这个就是json的一个例子

    ``` csharp
    // Deep Clone 实现
    JsonSerializer.DeserializeFromString<T>(JsonSerializer.SerializeToString<T>(obj))
    ```

* AutoMapper 对象映射库

    ``` csharp
    // Deep Clone 实现
    Mapper.Initialize(cfg => cfg.CreateMap<Student, Student>());
    var dest = Mapper.Map<Student, Student>(s);
    ```

在Release模式下，跑了1000000 次的 Deep Clone 性能测试如下

![性能测试.img](https://raw.githubusercontent.com/fs7744/DeepCloneExample/master/img/test.png)

简陋的测试代码参阅 ：https://github.com/fs7744/DeepCloneExample/blob/master/DeepClone/DeepClone/Program.cs

其实来说都是比较高效的，因为没有那个项目动不动就 Deep Clone 上百万次，

大家喜欢用什么就用什么，

比如 做 api service 有用 json序列化，而且有用了 ignoreJson 之类的东西，这时候 Deep Clone 可能不好用 json序列化了，那我们可以选用上述的其他方式，简单方便

上述所有内容参阅： https://github.com/fs7744/DeepCloneExample