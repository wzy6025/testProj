namespace list15;
using System.IO;

/// <summary>
/// 临时文件流类，用于创建和自动清理临时文件
/// </summary>
public class TemporaryFileStream
{
    /// <summary>
    /// 使用指定的文件名初始化TemporaryFileStream实例
    /// </summary>
    /// <param name="fileName">临时文件的文件名</param>
    public TemporaryFileStream(string fileName)
    {
        File = new FileInfo(fileName);
        // 更优的解决方案是使用FileOptions.DeleteOnClose选项
        Stream = new FileStream(
            File.FullName, FileMode.OpenOrCreate,
            FileAccess.ReadWrite);
            //FileShare.None,//用于指定其他进程或当前进程中的其他线程对同一个文件的共享访问权限。
            //4096,  //缓冲区大小（buffer size）
            //FileOptions.DeleteOnClose);
        //这样创建的 FileStream，在调用 Stream.Dispose() 或 Stream.Close() 后，文件会自动被删除。
        //无需在 Close() 方法中手动调用 File.Delete()，可以简化资源管理。

        //FileStream的对应构造函数：public FileStream(
            //string path, FileMode mode,
            //FileAccess access,
            //FileShare share,
            //int bufferSize,
            //FileOptions options);
    }

    /// <summary>
    /// 初始化TemporaryFileStream实例，自动生成临时文件名
    /// </summary>
    public TemporaryFileStream()
        : this(Path.GetTempFileName())
    { }

    /// <summary>
    /// finalizer析构函数，确保在对象被回收时关闭并清理临时文件
    /// </summary>
    ~TemporaryFileStream()  //finalizer
    {
        try
        {
            Close();//自定义的方法，见下面代码
        }
        catch (Exception)
        {
            // 可以在这里将错误写入日志或显示到UI
            // ...
        }
    }

    /// <summary>
    /// 获取文件流对象
    /// </summary>
    public FileStream? Stream { get; private set; }

    /// <summary>
    /// 获取文件信息对象
    /// </summary>
    public FileInfo? File { get; private set; }

    /// <summary>
    /// 关闭文件流并删除临时文件
    /// </summary>
    public void Close()
    {
        // 释放文件流资源
        Stream?.Dispose();//dispos=release释放
        try
        {
            // 删除临时文件
            File?.Delete();  //
        }
        catch (IOException exception)
        {
            Console.WriteLine(exception);
        }
        // 清空引用
        Stream = null;
        File = null;
    }
}
class Program
{
    static void Main(string[] args)
    {
        TemporaryFileStream tempFile = new("a.txt");
        tempFile.Stream?.WriteByte(0x20);
        Console.WriteLine($"{tempFile.File?.FullName}");
        Console.WriteLine($"{tempFile.Stream?.ToString()}");
        Console.ReadKey();
    }
}

//对TemporaryFileStream类的解释：

//public class TemporaryFileStream
//{
//    // 带文件名参数的构造函数
//    public TemporaryFileStream(string fileName)
//    {
//        File = new FileInfo(fileName);  // 创建文件信息对象
//        // 注：更优方案可使用FileOptions.DeleteOnClose（关闭时自动删除）选项
//        Stream = new FileStream(
//            File.FullName,  // 文件完整路径
//            FileMode.OpenOrCreate,  // 打开或创建文件的模式
//            FileAccess.ReadWrite);  // 读写文件的访问权限
//    }

//    // 无参数构造函数（调用带参构造函数，创建临时文件）
//    public TemporaryFileStream()
//        : this(Path.GetTempFileName())  // 生成临时文件名并传入带参构造函数
//    { }

//    // 终结器（对象被回收前自动调用）
//    ~TemporaryFileStream()
//    {
//        try
//        {
//            Close();  // 调用关闭方法清理资源
//        }
//        catch (Exception)
//        {
//            // 此处可将异常信息写入日志或显示到界面
//            // ...
//        }
//    }

//    // 公开的文件流属性（仅可读，私有 setter 确保外部无法修改）
//    public FileStream? Stream { get; private set; }
//    // 公开的文件信息属性（仅可读，私有 setter 确保外部无法修改）
//    public FileInfo? File { get; private set; }

//    // 关闭方法（手动触发资源清理）
//    public void Close()
//    {
//        Stream?.Dispose();  // 释放文件流资源（若Stream不为null）
//        try
//        {
//            File?.Delete();  // 删除文件（若File不为null）
//        }
//        catch (IOException exception)  // 捕获IO异常（如文件被占用）
//        {
//            Console.WriteLine(exception);  // 输出异常信息
//        }
//        Stream = null;  // 置空引用，帮助垃圾回收器识别可回收资源
//        File = null;    // 置空引用，帮助垃圾回收器识别可回收资源
//    }
//}