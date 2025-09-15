namespace ResourceWrapper;

public class ResourceWrapper : IDisposable
{
    // 模拟一个非托管资源
    private IntPtr _unmanagedResource;

    // 模拟一个托管资源
    private ManagedResource _managedResource;

    // 跟踪是否已释放资源
    private bool _disposed = false;

    public ResourceWrapper()
    {
        // 初始化资源
        _unmanagedResource = AllocateUnmanagedResource();
        _managedResource = new ManagedResource();
        Console.WriteLine("资源已初始化");
    }

    // 模拟分配非托管资源
    private IntPtr AllocateUnmanagedResource()
    {
        return new IntPtr(123); // 仅作示例
    }

    // 模拟释放非托管资源
    private void FreeUnmanagedResource(IntPtr resource)
    {
        // 实际应用中会释放非托管资源
        Console.WriteLine("非托管资源已释放");
    }

    // 实现IDisposable接口的Dispose方法
    public void Dispose()
    {
        Dispose(true);
        // 告诉GC不需要再调用终结器
        GC.SuppressFinalize(this);
    }

    // 核心释放逻辑
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        // 如果是主动调用Dispose()，释放托管资源
        if (disposing)
        {
            if (_managedResource != null)
            {
                _managedResource.Dispose(); // 释放托管资源
                Console.WriteLine("托管资源已释放");
                _managedResource = null;
            }
        }

        // 无论何种方式，都释放非托管资源
        if (_unmanagedResource != IntPtr.Zero)
        {
            FreeUnmanagedResource(_unmanagedResource);
            _unmanagedResource = IntPtr.Zero;
        }

        _disposed = true;
    }

    // 终结器：作为释放资源的备份机制
    ~ResourceWrapper()
    {
        Dispose(false);
        Console.WriteLine("终结器执行资源释放");
    }

    // 其他业务方法
    public void DoWork()
    {
        if (_disposed)
            throw new ObjectDisposedException("ResourceWrapper", "对象已释放，无法执行操作");

        Console.WriteLine("执行操作...");
    }
}

// 模拟一个需要释放的托管资源
public class ManagedResource : IDisposable
{
    private bool _disposed = false;

    public void Dispose()
    {
        if (!_disposed)
        {
            // 释放托管资源的逻辑
            _disposed = true;
        }
    }
}

// 使用示例
public class Program
{
    public static void Main()
    {
        // 使用using语句确保资源自动释放
        using (var resource = new ResourceWrapper())
        {
            resource.DoWork();
        } // 此处自动调用Dispose()

        Console.WriteLine("操作完成");
    }
}
