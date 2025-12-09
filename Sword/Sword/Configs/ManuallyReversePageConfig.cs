using Newtonsoft.Json.Linq;
using SSMT;
using SSMT_Core;
using Sword.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Configs
{
    public static class ManuallyReversePageConfig
    {

        public static List<IndexBufferItem> IndexBufferItemList { get; set; } = new List<IndexBufferItem>();

        public static List<CategoryBufferItem> CategoryBufferItemList { get; set; } = new List<CategoryBufferItem>();   

        public static List<ShapeKeyPositionBufferItem> ShapeKeyPositionBufferItemList { get; set; } = new List<ShapeKeyPositionBufferItem>();

        public static string GameTypeName { get; set; } = "GIMI";

        public static void ReadConfig()
        {
            if (!File.Exists(PathManager.Path_ManuallyReversePageConfig))
            {
                return;
            }

            JObject ManuallyReversePageConfigJOBJ = DBMTJsonUtils.ReadJObjectFromFile(PathManager.Path_ManuallyReversePageConfig);

            if (ManuallyReversePageConfigJOBJ.ContainsKey("GameTypeName"))
            {
                GameTypeName = (string)ManuallyReversePageConfigJOBJ["GameTypeName"];
            }

            if (ManuallyReversePageConfigJOBJ.ContainsKey("IndexBufferItemList"))
            {
                JArray IndexBufferItemListJarray = (JArray)ManuallyReversePageConfigJOBJ["IndexBufferItemList"];

                IndexBufferItemList.Clear();
                foreach (JObject jobj in IndexBufferItemListJarray)
                {
                    IndexBufferItem indexBufferItem = new IndexBufferItem();
                    indexBufferItem.Format = (string)jobj["Format"];
                    indexBufferItem.IBFilePath = (string)jobj["IBFilePath"];
                    IndexBufferItemList.Add(indexBufferItem);
                }
            }

            if (ManuallyReversePageConfigJOBJ.ContainsKey("CategoryBufferItemList"))
            {
                JArray CategoryBufferItemListJarray = (JArray)ManuallyReversePageConfigJOBJ["CategoryBufferItemList"];

                CategoryBufferItemList.Clear();
                foreach (JObject jobj in CategoryBufferItemListJarray)
                {
                    CategoryBufferItem categoryBufferItem = new CategoryBufferItem();
                    categoryBufferItem.Category = (string)jobj["Category"];
                    categoryBufferItem.BufFilePath = (string)jobj["BufFilePath"];
                    CategoryBufferItemList.Add(categoryBufferItem);
                }
            }

            if (ManuallyReversePageConfigJOBJ.ContainsKey("ShapeKeyPositionBufferItemList"))
            {
                JArray ShapeKeyPositionBufferItemListJarray = (JArray)ManuallyReversePageConfigJOBJ["ShapeKeyPositionBufferItemList"];

                ShapeKeyPositionBufferItemList.Clear();
                foreach (JObject jobj in ShapeKeyPositionBufferItemListJarray)
                {
                    ShapeKeyPositionBufferItem shapeKeyPositionBufferItem = new ShapeKeyPositionBufferItem();
                    shapeKeyPositionBufferItem.Category = (string)jobj["Category"];
                    shapeKeyPositionBufferItem.BufFilePath = (string)jobj["BufFilePath"];
                    ShapeKeyPositionBufferItemList.Add(shapeKeyPositionBufferItem);
                }
            }

        }

        public static void SaveConfig()
        {
            JObject ManuallyReversePageConfigJOBJ = new JObject();

            ManuallyReversePageConfigJOBJ["GameTypeName"] = GameTypeName;

            //IndexBufferItemList
            JArray IndexBufferItemListJarray = new JArray();
            foreach (IndexBufferItem indexBufferItem in IndexBufferItemList)
            {
                JObject jobj = new JObject();
                jobj["Format"] = indexBufferItem.Format;
                jobj["IBFilePath"] = indexBufferItem.IBFilePath;
                IndexBufferItemListJarray.Add(jobj);
            }
            ManuallyReversePageConfigJOBJ["IndexBufferItemList"] = IndexBufferItemListJarray;

            //CategoryBufferItemList
            JArray CategoryBufferItemListJarray = new JArray();
            foreach (CategoryBufferItem categoryBufferItem in CategoryBufferItemList)
            {
                JObject jobj = new JObject();
                jobj["Category"] = categoryBufferItem.Category;
                jobj["BufFilePath"] = categoryBufferItem.BufFilePath;
                CategoryBufferItemListJarray.Add(jobj);
            }
            ManuallyReversePageConfigJOBJ["CategoryBufferItemList"] = CategoryBufferItemListJarray;

            //ShapeKeyPositionBufferItemList
            JArray ShapeKeyPositionBufferItemListJarray = new JArray();
            foreach (ShapeKeyPositionBufferItem shapeKeyPositionBufferItem in ShapeKeyPositionBufferItemList)
            {
                JObject jobj = new JObject();
                jobj["Category"] = shapeKeyPositionBufferItem.Category;
                jobj["BufFilePath"] = shapeKeyPositionBufferItem.BufFilePath;
                ShapeKeyPositionBufferItemListJarray.Add(jobj);
            }
            ManuallyReversePageConfigJOBJ["ShapeKeyPositionBufferItemList"] = ShapeKeyPositionBufferItemListJarray;


            DBMTJsonUtils.SaveJObjectToFile(ManuallyReversePageConfigJOBJ, PathManager.Path_ManuallyReversePageConfig);
        }

    }

}
