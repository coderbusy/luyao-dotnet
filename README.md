# LuYao .NET ͨ�ù��߿�

[![MIT License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/LuYao.Common.svg)](https://www.nuget.org/packages/LuYao.Common)

## ���

`luyao-dotnet` ��һ�������������õ� .NET ���߿⣬�ṩ�ḻ��ͨ�ù��������չ���������ǻ��桢��ϣ�����顢�߳�������ʱ�ļ����ַ���ѹ���ȳ��ÿ������������� .NET ��������������Ч�������������

## ��Ҫ����

- **�������**��`CachedObject<T>` ֧�ֻ���ֵ�����ֹ��ڲ��ԡ�
- **���鹤��**��`Arrays` ���ݻ�ȡָ�����͵Ŀ����顣
- **��ϣ�㷨**��`HashAgent`/`HashAgents` ֧�� CRC32��CRC64��MD5��SHA1 �ȳ��ù�ϣ�㷨��
- **��ʱ�ļ�����**��`AutoCleanTempFile` �����Զ�������ʱ�ļ���������Դй¶��
- **�߳�������**��`KeyedLocker<T>` ʵ�ֻ��� Key �ĸ߲�������
- **�ַ���ѹ��/��ѹ**��`LzString`��`GZipString` ֧�ֶ����ַ���ѹ�����ѹ���㷨��

## ��װ

�Ƽ�ͨ�� NuGet ��װ��

```bash
dotnet add package LuYao.Common
```
����� [NuGet ����](https://www.nuget.org/packages/LuYao.Common) ��ȡ������Ϣ��

Ҳ��ͨ��Դ�뼯�ɵ���� .NET ��Ŀ�У�

```bash
git clone https://github.com/coderbusy/luyao-dotnet.git
```

## ʾ��

```csharp
// ��������ʹ��
var cache = new CachedObject<string>("Hello", TimeSpan.FromMinutes(5));
Console.WriteLine(cache.Value);

// ��ȡ������
var emptyInts = Arrays.Empty<int>();

// �����ļ��� MD5
var md5 = HashAgents.MD5.HashFile("test.txt");

// �����Զ��������ʱ�ļ�
using var tempFile = new AutoCleanTempFile();
File.WriteAllText(tempFile.FileName, "��ʱ����");

// �߳�������
var lockObj = KeyedLocker<string>.GetLock("my-key");
lock (lockObj)
{
    // �ٽ�������
}
```

## ��ȨЭ��

����Ŀ���� [MIT License](LICENSE) ��Դ����ӭ����ʹ���빱�ס�

---

## ����

��ӭ�ύ Issue �� PR��

---

## ��ϵ

GitHub: [coderbusy/luyao-dotnet](https://github.com/coderbusy/luyao-dotnet)  
NuGet: [LuYao.Common](https://www.nuget.org/packages/LuYao.Common)
