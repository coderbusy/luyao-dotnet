<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.IO.Compression</Namespace>
</Query>

async Task Main()
{
	var url = "https://github.com/fake-useragent/fake-useragent/raw/refs/heads/main/src/fake_useragent/data/browsers.jsonl";
	var http = new HttpClient();
	var output = await http.GetStringAsync(url);
	var lines = output.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
	var items = new List<BrowserItem>();
	var types = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
	var brands = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
	var browser = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
	var os = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
	var platforms = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
	foreach (var line in lines)
	{
		var dto = JsonConvert.DeserializeObject<BrowserItem>(line);
		if (dto == null || string.IsNullOrWhiteSpace(dto.UserAgent)) continue;
		items.Add(dto);
		if (!string.IsNullOrWhiteSpace(dto.Type)) types.Add(dto.Type);
		if (!string.IsNullOrWhiteSpace(dto.DeviceBrand)) brands.Add(dto.DeviceBrand);
		if (!string.IsNullOrWhiteSpace(dto.Browser)) browser.Add(dto.Browser);
		if (!string.IsNullOrWhiteSpace(dto.Os)) os.Add(dto.Os);
		if (!string.IsNullOrWhiteSpace(dto.Platform)) platforms.Add(dto.Platform);
	}
	types.Dump();
	browser.Dump();
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var fn = Path.Combine(dir, "Browsers.dat");
	using (var fs = File.OpenWrite(fn))
	using (var gz = new GZipStream(fs, CompressionLevel.Optimal))
	using (var sw = new BinaryWriter(gz, Encoding.UTF8))
	{
		int len = items.Count;
		sw.Write(len);
		foreach (var item in items)
		{
			item.Write(sw);
		}
	}
}
public partial class BrowserItem
{
	[JsonProperty("useragent")]
	public string UserAgent { get; set; }

	[JsonProperty("percent")]
	public double Percent { get; set; }

	[JsonProperty("type")]
	public string Type { get; set; }

	[JsonProperty("device_brand")]
	public string DeviceBrand { get; set; }

	[JsonProperty("browser")]
	public string Browser { get; set; }

	[JsonProperty("browser_version")]
	public string BrowserVersion { get; set; }

	[JsonProperty("os")]
	public string Os { get; set; }

	[JsonProperty("os_version")]
	public string OsVersion { get; set; }

	[JsonProperty("platform")]
	public string Platform { get; set; }

	public void Write(BinaryWriter w)
	{
		w.Write(this.UserAgent);
		w.Write(this.Percent);
		w.Write(this.Type);
		w.Write(this.DeviceBrand ?? string.Empty);
		w.Write(this.Browser);
		w.Write(this.BrowserVersion);
		w.Write(this.Os);
		w.Write(this.OsVersion);
		w.Write(this.Platform);
	}
}
