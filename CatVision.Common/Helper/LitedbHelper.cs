using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using CatVision.Common.Enum;
using CatVision.Common.Model;

namespace CatVision.Common.Helper
{
    public class LitedbHelper
    {
        /// <summary>
        /// bsonvalue: int32 int64 double decimal string document array binary=byte[] guid boolean datetime
        /// </summary>
        string connStr = Path.Combine(Directory.GetCurrentDirectory(), "config", "user.db");
    }
    /// <summary>
    /// 项目文件数据库
    /// </summary>
    public class ProjectDb : IDisposable
    {
        private static ProjectDb instance;
        public static ProjectDb Ins => instance;
        public bool Connectted { get; } = false;
        public string FoldTemp { get; } = "$/temp/"; /// 临时目录
        public string FoldCsharp { get; } = "$/csharp/"; /// C# 算法脚本存储目录
        public string FoldDll { get; } = "$/dll/"; /// 编译后的算法dll存储目录
        public string FoldTrigger { get; } = "$/trigger/"; /// C# 流程发脚本存储目录
        // public string ColGolbalVal { get; } = "golbal_val"; /// 全局变量表
        public string ColGlobalVal { get; } = "$/global_val/"; /// 全局变量存储目录
        // public string ColDevicePara { get; } = "device_para"; /// 连接设备信息表
        public string ColDevicePara { get; } = "$/device_para/"; /// 连接设备信息存储目录
        // public string ColJsonPara { get; } = "json_para"; /// 产品参数表
        public string ColProductPara { get; } = "$/product_para/"; /// 产品参数表
        // public string ColCamPara { get; } = "cam_para"; /// 相机参数表 : TODO 是否被包含于产品参数表中，当前考虑：保留单独配置，载入参数时校验相机
        public string ColCamPara { get; } = "$/cam_para/"; /// 相机参数存储目录

        private string DbContext; /// 连接字符串 TODO: 登陆密码 & 加密
        private LiteDatabase Db; /// 数据库
        public ProjectDb()
        {
            instance = new ProjectDb();
            DbContext = Path.Combine(Directory.GetCurrentDirectory(), "config", "default.pro");
        }
        public ProjectDb(string connStr)
        {
            DbContext = connStr;
            instance = this;
        }
        public void CreateProject(string connStr)
        {
            if (connStr == DbContext || string.IsNullOrEmpty(connStr))
            {
                if (Db is null) Db = new LiteDatabase(DbContext);
            }
            else
            {
                if (Db is null)
                {
                    Db.Checkpoint();
                    Db.Dispose();
                }
                Log.Default.Warn(@"ProjectDb {0} closed.", DbContext);
                DbContext = connStr;
                Db = new LiteDatabase(DbContext);
                Log.Default.Info(@"ProjectDb {0} opened.", DbContext);
            }
        }
        public void DumpDbFile(string fullname)
        {
            if (Db is null) return;
            Db.Checkpoint();
            Db.Dispose();
            File.Copy(DbContext, fullname);
            Db = new LiteDatabase(DbContext);
        }
        /// <summary>
        /// TODO: necessary
        /// </summary>
        private void MapperLiteDb()
        {
            //BsonMapper.Global.Entity<Order>()DbRef(x => x.Customer, "customers");
            //mapper.Entity<MyEntity>()
            //.Id(x => x.MyCustomKey) // set your document ID
            //.Ignore(x => x.DoNotSerializeThis) // ignore this property (do not store)
            //.Field(x => x.CustomerName, "cust_name"); // rename document field
            /*BsonMapper.Global.RegisterType<UserModel>(
                serialize: obj => new BsonDocument(
                    new Dictionary<string, BsonValue> {
                        { "userlevel", obj.Userlevel}, { "username", obj.UserName}, { "userpwd",  obj.UserPwd} }),
                /*{
                    var doc = new BsonDocument();
                    doc["userlevel"] = obj.Userlevel;
                    doc["username"] = obj.UserName;
                    doc["userpwd"] = obj.UserPwd;
                    return doc;
                },
                deserialize: doc => new UserModel(doc["userlevel"], doc["username"], doc["userpwd"])
            );*/
        }

        /// <summary>
        /// 文件存储相关操作
        /// example: name = "myclass.cs"; id = "$/csharp/myclass.cs"
        /// </summary>
        public string GetFileID(string name, bool csharp = false, bool dll = false, bool trigger = false, bool global_val = false,
            bool device_para = false, bool product_para = false, bool cam_para = false)
        {
            string id = name;
            if (csharp) id = FoldCsharp + name;
            else if (dll) id = FoldDll + name;
            else if (trigger) id = FoldTrigger + name;
            else if (global_val) id = ColGlobalVal + name;
            else if (device_para) id = ColDevicePara + name;
            else if (product_para) id = ColProductPara + name;
            else if (cam_para) id = ColCamPara + name;
            else id = FoldTemp + name;
            return id;
        }
        public void UploadFile(string name, Stream stream, bool csharp = false, bool dll = false, bool trigger = false, bool global_val = false,
            bool device_para = false, bool product_para = false, bool cam_para = false)
        {
            Db.FileStorage.Upload(GetFileID(name, csharp, dll, trigger, global_val, device_para, product_para, cam_para), name, stream);
        }
        public void DownloadFile(string name, Stream stream, bool csharp = false, bool dll = false, bool trigger = false, bool global_val = false,
            bool device_para = false, bool product_para = false, bool cam_para = false)
        {
            Db.FileStorage.FindById(GetFileID(name, csharp, dll, trigger, global_val, device_para, product_para, cam_para)).CopyTo(stream);
        }
        public void DeleteFile(string name, bool csharp = false, bool dll = false, bool trigger = false, bool global_val = false,
            bool device_para = false, bool product_para = false, bool cam_para = false)
        {
            Db.FileStorage.Delete(GetFileID(name, csharp, dll, trigger, global_val, device_para, product_para, cam_para));
        }

        /// <summary>
        /// 类的增删改查
        /// </summary>
        public void Create<T>(string colName) { Db.GetCollection<T>(colName); }
        public void Insert(string colName, BsonDocument doc) { Db.GetCollection(colName).Insert(doc); }
        public void Insert<T>(T t) { Db.GetCollection<T>().Insert(t); }
        public void Delete<T>(Expression<Func<T, bool>> exp) { Db.GetCollection<T>().DeleteMany(exp); }
        public void DeleteAll<T>() { Db.GetCollection<T>().DeleteAll(); }
        public void Update<T>(T t) { Db.GetCollection<T>().Update(t); }
        public List<T> Find<T>(Expression<Func<T, bool>> exp) { return Db.GetCollection<T>().Find(exp).ToList(); }
        public List<T> FindAll<T>() { return Db.GetCollection<T>().FindAll().ToList(); }
        /// <summary>
        /// 辅助方法
        /// </summary>
        public static string DumpToJsonStr(object obj)
        {
            return BsonMapper.Global.ToDocument(obj).ToString();
        }
        public static T SetFromJsonStr<T>(string json)
        {
            return BsonMapper.Global.ToObject<T>(JsonSerializer.Deserialize(json).AsDocument);
        }
        public string GetFilePath()
        {
            return DbContext;
        }
        public void SaveAs(string fullpath)
        {
            Db.Checkpoint();
            File.Copy(DbContext, fullpath);
        }
        public void Dispose()
        {
            if (!(Db is null))
            {
                Db.Checkpoint();
                Db.Dispose();
            }
        }
    }
    public class UserDBHelper : IDisposable
    {
        private static UserDBHelper instance = new UserDBHelper();
        public static UserDBHelper Ins => instance;

        public string ColName { get; } = "user";
        private string DbContext;
        private LiteDatabase UserDb;
        private ILiteCollection<UserModel> Col;
        public UserDBHelper()
        {
            DbContext = Path.Combine(Directory.GetCurrentDirectory(), "config", "user.db");
            UserDb = new LiteDatabase(DbContext);
            MapperLiteDb();
            Col = UserDb.GetCollection<UserModel>(ColName);
            Col.EnsureIndex(x => x.UserName, true);
        }
        public void Insert(UserModel user) { Col.Insert(user); }
        public void Delete(string username) { Col.DeleteMany(x => x.UserName == username); }
        public List<UserModel> ReadAll() { return Col.FindAll().ToList(); }
        public List<UserModel> ReadAllLevel(uint level) { return Col.Find(x => x.Userlevel == ((UserLevel)level).ToString()).ToList(); }
        private void MapperLiteDb()
        {
            BsonMapper.Global.RegisterType<UserModel>(
                serialize: obj => new BsonDocument(
                    new Dictionary<string, BsonValue> {
                        { "userlevel", obj.Userlevel}, { "username", obj.UserName}, { "userpwd",  obj.UserPwd} }),
                deserialize: doc => new UserModel(doc["userlevel"], doc["username"], doc["userpwd"])
            );
        }
        public void Dispose()
        {
            if (!(UserDb is null)) UserDb.Dispose(); 
        }
    }

    public class ResultDBHelper : IDisposable
    {
        private static ResultDBHelper instance;
        public static ResultDBHelper Ins => instance;
        public string DbContext;
        public string ColName { get; } = "data";
        public Action<string> OnDataChanged;
        private LiteDatabase ResultDb;
        private ILiteCollection<ResultData> Col;
        public ResultDBHelper()
        {
            DbContext = Path.Combine(Directory.GetCurrentDirectory(), "data", "data.db");
            ResultDb = new LiteDatabase(DbContext);
            MapperLiteDb();
            Col = ResultDb.GetCollection<ResultData>(ColName);
            Col.EnsureIndex(x => x.datetime, true);
            instance = this;
        }
        public static void MapperLiteDb()
        {
            BsonMapper.Global.RegisterType<ResultData>(
                serialize: obj => new BsonDocument()
                { {"datetime", obj.datetime }, {"camera", obj.camera }, {"product", obj.product} },
                deserialize: doc => new ResultData 
                { datetime = doc["datetime"], camera = doc["camera"], product = doc["product"] }
            );
        }
        public void Insert(ResultData res) { Col.Insert(res); OnDataChanged?.BeginInvoke("Insert", null, null); }
        public List<ResultData> Read(System.Linq.Expressions.Expression<Func<ResultData, bool>> exp, int skipCount = 0, int pageSize = 50)
        { return Col.Find(exp, skipCount, pageSize).ToList(); }
        public List<ResultData> ReadOneDay(DateTime date, int skipCount = 0, int pageSize = 50)
        // todo: bug: will Paginate first and order next
        //{ return Col.Find((x => x.datetime.Date == date.Date), skipCount, pageSize).OrderByDescending(x => x.datetime).ToList(); }
        { return Col.Find(x => x.datetime.Date == date.Date).OrderByDescending(x => x.datetime).Skip(skipCount).Take(pageSize).ToList(); }
        public ResultData ReadNewest() // todo DESC does not take effict
        { return Col.Find(Query.All("$$DESC"), limit: 1).FirstOrDefault(); }
        public List<ResultData> ReadAll(int skipCount = 0, int pageSize = Int32.MaxValue - 1)
        { var a = Col.Find(Query.All("$$DESC"), skipCount, pageSize).ToList(); return a; }
        public Dictionary<DateTime, int> CountDaily()
        {
            return Col.FindAll().GroupBy(x => x.datetime.Date)
                .Select(g => new { Date = g.Key, DataCount = g.Count() })
                .OrderBy(x => x.Date).ToDictionary(x => x.Date, x => x.DataCount);
        }
        public Dictionary<DateTime, int> CountDaily(string camname)
        {
            return Col.Find(x => x.camera == camname).GroupBy(x => x.datetime.Date)
                .Select(g => new { Date = g.Key, DataCount = g.Count() })
                .OrderBy(x => x.Date).ToDictionary(x => x.Date, x => x.DataCount);
        }
        public List<string> CountCamname()
        {
            return Col.FindAll().Select(x => x.camera).Distinct().ToList();
        }
        public Dictionary<string, int> CountCamera()
        {
            return Col.FindAll().GroupBy(r => r.camera)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderBy(x => x.Name).ToDictionary(x => x.Name, x => x.Count);
        }
        public Dictionary<string, int> CountProduct()
        {
            return Col.FindAll().GroupBy(r => r.product)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderBy(x => x.Name).ToDictionary(x => x.Name, x => x.Count);
        }
        public int Delete(System.Linq.Expressions.Expression<Func<ResultData, bool>> exp) { return Col.DeleteMany(exp); }
        public int Delete(DateTime dt) { return Col.DeleteMany(x => x.datetime == dt); }
        public int DeleteAll() { return Col.DeleteAll(); }
        public void UploadFile(int id, string filename) { ResultDb.GetStorage<int>().Upload(id, filename); }
        public void DownloadFile(int id, string filename) { ResultDb.GetStorage<int>().Download(id, filename, true); }
        public static void DumpOneToCsv(ResultData res, string path, string name)
        {
            CsvHelper.writeData(name, path, res.GetStr());
        }
        public static void DumpToCsv(IEnumerable<ResultData> res, string path, string name, string[] header, bool useHead)
        {
            CsvHelper.CreateCSV(name, path, header, useHead);
            using (StreamWriter sw = new StreamWriter(Path.Combine(path, name), true, Encoding.GetEncoding("gb2312")))
            {
                foreach (var item in res)
                {
                    sw.WriteLine(item.GetSpliteStr(","));
                }
            }
        }
        public void Dispose() { ResultDb.Dispose(); }
    }
}
