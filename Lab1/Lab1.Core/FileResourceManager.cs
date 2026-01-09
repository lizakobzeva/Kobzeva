using System.Text;

namespace Lab1.Core;

public class FileResourceManager : IDisposable
{
    private FileStream _fileStream;
    private StreamWriter _writer;
    private StreamReader _reader;
    private bool _disposed = false;
    private readonly string _filePath;

    public FileResourceManager(string filePath, FileMode fileMode)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        
        try
        {
            _fileStream = new FileStream(filePath, fileMode, FileAccess.ReadWrite, FileShare.Read);
        }
        catch (Exception ex)
        {
            throw new IOException($"не получилось открыть файл: {filePath}", ex);
        }
    }

    public void OpenForWriting()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FileResourceManager));
        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"файл не найден: {_filePath}");
        if (_writer != null)
            _writer.Dispose();
        
        _writer = new StreamWriter(_fileStream, Encoding.UTF8);
    }

    public void OpenForReading()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FileResourceManager));
        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"файл не найден: {_filePath}");
        if (_reader != null)
            _reader.Dispose();
        
        _fileStream.Position = 0;
        _reader = new StreamReader(_fileStream, Encoding.UTF8);
    }

    public void WriteLine(string text)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FileResourceManager));
        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"файл не найден: {_filePath}");
        if (_writer == null)
            OpenForWriting();

        try
        {
            _writer.WriteLine(text);
            _writer.Flush();
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    public string ReadAllText()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FileResourceManager));
        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"файл не найден: {_filePath}");
        if (_reader == null)
            OpenForReading();
        
        try
        {
            return _reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    public void AppendText(string text)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FileResourceManager));
        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"файл не найден: {_filePath}");
        if (_writer == null)
            OpenForWriting();

        try
        {
            _fileStream.Position = _fileStream.Length;
            _writer.Write(text);
            _writer.Flush();
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    public string GetFileInfo()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FileResourceManager));
        if (!File.Exists(_filePath))
            throw new FileNotFoundException($"файл не найден: {_filePath}");
        
        try
        {
            FileInfo fi = new FileInfo(_filePath);
            return $"Размер: {fi.Length} байт\nВремя создания: {fi.CreationTime}\nПоследнее изменение: {fi.LastWriteTime}";
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            DisposeResources();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~FileResourceManager()
    {
        Dispose(false);
    }
    
    

    private void DisposeResources()
    {
        _writer?.Dispose();
        _reader?.Dispose();
        _fileStream?.Dispose();

        _writer = null;
        _reader = null;
        _fileStream = null;
    }

    private void LogError(Exception ex)
    {
        try
        {
            File.AppendAllText("error.log", $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n");
        }
        catch { /* пропуск ошибок при логировании */ }
    }
}

