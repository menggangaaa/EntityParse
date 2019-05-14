﻿using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace EntityParse
{
    public partial class mainPanel : Form
    {

        AutoSizeFormClass asc = new AutoSizeFormClass();
        TableDataLink thisTableNode;

        public mainPanel()
        {
            InitializeComponent();
        }
        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filepath);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder returnvalue, int buffersize, string filepath);

        [DllImport("kernel32.dll")]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, byte[] lpReturnedString, uint nSize, string lpFileName);

        private string basePath = "";//基本信息配置路径

        private string entityPath = "";//实体配置路径
        private string relationPath = "";//关系配置路径
        private string enumPath = "";//枚举配置路径
        private string bizunitPath = "";//业务单元配置路径
        private string packagePath = "";//包配置路径

        private string entityToPackagePath = "";//实体对应包路径配置路径
        private string logPath = "";//日志路径

        //private void GetValue(string section, string key, out string value)

        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    GetPrivateProfileString(section, key, "", stringBuilder, 1024, basePath);
        //    value = stringBuilder.ToString();
        //}
        private void GetValue(string section, string key, out string value, string path)

        {
            StringBuilder stringBuilder = new StringBuilder();
            GetPrivateProfileString(section, key, "", stringBuilder, 1024, path);
            value = stringBuilder.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "选择项目根目录";
            folder.SelectedPath = textBox1.Text;
            if (folder.ShowDialog() == DialogResult.OK)
            {
                //\\scm
                if (radioButton1.Checked)
                {
                    if (Directory.Exists(folder.SelectedPath + "\\metadata\\com\\kingdee\\eas\\hse\\scm") == true)
                    {
                        textBox1.Text = folder.SelectedPath;
                        WritePrivateProfileString("Information", "path", textBox1.Text.Trim(), basePath);
                        if (radioButton1.Checked)
                        {
                            WritePrivateProfileString("Information", "select", "1", basePath);
                        }
                        else
                        {
                            WritePrivateProfileString("Information", "select", "2", basePath);
                        }
                        //+ "\\metadata\\com\\kingdee\\eas\\hse\\scm";
                        initTree();
                    }
                    else
                    {
                        MessageBox.Show("选择的文件夹不是项目的根目录,请重新选择!");
                    }
                }
                else
                {
                    if (Directory.Exists(folder.SelectedPath + "\\metas") == true)
                    {
                        textBox1.Text = folder.SelectedPath;
                        WritePrivateProfileString("Information", "path", textBox1.Text.Trim(), basePath);
                    }
                    else
                    {
                        MessageBox.Show("选择的文件夹不是客户端的根目录,请重新选择!");
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            initTree();
        }

        private void fileExists(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
        }

        private void mainPanel_Load(object sender, EventArgs e)
        {
            basePath = Application.LocalUserAppDataPath + "\\BaseConfig.ini";//基本信息配置路径
            entityPath = Application.LocalUserAppDataPath + "\\EntityConfig.ini";//实体配置路径
            relationPath = Application.LocalUserAppDataPath + "\\RealtionConfig.ini";//关系配置路径
            enumPath = Application.LocalUserAppDataPath + "\\EnumConfig.ini";//枚举配置路径
            bizunitPath = Application.LocalUserAppDataPath + "\\BizunitConfig.ini";//业务单元配置路径
            packagePath = Application.LocalUserAppDataPath + "\\PackageConfig.ini";//包配置路径
            entityToPackagePath = Application.LocalUserAppDataPath + "\\EntityToPackageConfig.ini";//实体对应包路径配置路径
            logPath = Application.LocalUserAppDataPath + "\\Log.ini";//实体对应包路径配置路径

            fileExists(basePath);
            fileExists(entityPath);
            fileExists(relationPath);
            fileExists(enumPath);
            fileExists(bizunitPath);
            fileExists(packagePath);
            fileExists(entityToPackagePath);
            fileExists(logPath);

            string outString;
            try
            {
                GetValue("Information", "path", out outString, basePath);
                if (outString != null && outString.Length > 0)
                {
                    textBox1.Text = outString;
                }
                GetValue("Information", "select", out outString, basePath);
                if (outString != null && outString.Length > 0)
                {
                    if (outString == "1")
                    {
                        radioButton1.Checked = true;
                    }
                    else
                    {
                        radioButton2.Checked = true;
                    }
                }
                GetValue("Information", "isInitJar", out outString, basePath);
                if (outString == null || outString.Length == 0 || outString == "0")
                {
                    //未初始化jar包列表,初始化jar包列表
                    //initJarMetaDice(null);
                    label2.Visible = true;
                    progressBar1.Visible = false;
                    progressBar1.Value = 0;
                }
                else
                {
                    label2.Visible = false;
                    progressBar1.Visible = true;
                    progressBar1.Value = 100;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
            //initJarMetaDice();
            //asc.controllInitializeSize(this);     \\scm
            string path = textBox1.Text + "\\metadata\\com\\kingdee\\eas\\hse\\scm";
            if (Directory.Exists(path) && radioButton1.Checked)
            {
                initTree();
            }
            List<string> jarMetas = ReadSingleSection("metadata", entityPath);
            foreach (string jarMetaName in jarMetas)
            {

                textBox3.AutoCompleteCustomSource.Add(jarMetaName);
            }
            jarMetas = ReadSingleSection("metadata", bizunitPath);
            foreach (string jarMetaName in jarMetas)
            {

                textBox3.AutoCompleteCustomSource.Add(jarMetaName);

            }
            textBox3.AutoCompleteMode = AutoCompleteMode.Suggest;
            textBox3.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox3.Focus();

            initSqlPanel();
        }

        public TextBox txtSql = null;
        public ToolStripDropDown dropDown = null;
        private void initSqlPanel()
        {
            txtSql = new TextBox();
            txtSql.Multiline = true;
            txtSql.WordWrap = false;
            txtSql.ScrollBars = ScrollBars.Both;
            txtSql.Font = new Font("微软雅黑", 12f);
            txtSql.Text = "select * from table";
            txtSql.BorderStyle = BorderStyle.None;
            txtSql.Margin = new Padding(0);
            ToolStripControlHost treeViewHost = new ToolStripControlHost(txtSql);
            treeViewHost.Margin = new Padding(0);
            treeViewHost.AutoSize = false;
            treeViewHost.Size = new Size(385, 445);
            dropDown = new ToolStripDropDown();
            dropDown.Margin = new Padding(0);
            dropDown.Items.Add(treeViewHost);
            //dropDown.Show(this, 880, 300);

            txtSql.KeyDown += TxtSql_KeyDown;
        }

        private void TxtSql_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }
        }

        private void mainPanel_SizeChanged(object sender, EventArgs e)
        {
            //asc.controlAutoSize(this);
        }

        private void mainPanel_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            xmlParse(path);
        }

        private void mainPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }


        private void xmlParse(string path)
        {
            XmlTextReader reader = null;
            if (!File.Exists(path))
            {
                reader = getJarMetaObject(path);
                if (reader == null)
                {
                    MessageBox.Show("文件不存在!");
                    return;
                }
            }
            else
            {
                reader = new XmlTextReader(path);
            }
            dataGridView1.Rows.Clear();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }

            textBox4.Text = "";
            Dictionary<string, string> dict = new Dictionary<string, string>();//字段名
            Dictionary<string, string> dict2 = new Dictionary<string, string>();//数据库表名
            Dictionary<string, string> dict3 = new Dictionary<string, string>();//关联关系表
            Dictionary<string, string> dict4 = new Dictionary<string, string>();//数据类型
            Dictionary<string, string> dict5 = new Dictionary<string, string>();//关联关系表-具体路径
            int n = 0;
            string baseEntity = "";

            EntityInfo info = new EntityInfo();
            try
            {

                while (reader.Read())
                {
                    string column = "";
                    string columnName = "";
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "baseEntity")
                        {
                            if (reader.ReadToDescendant("key"))
                            {
                                string pack = reader.GetAttribute("value");
                                if (reader.ReadToNextSibling("key"))
                                {
                                    string name = reader.GetAttribute("value");
                                    baseEntity = getMetaDataFullPath(pack + "." + name, ".entity");
                                }
                            }
                        }
                        else if (reader.Name == "name")
                        {
                            if (info.name == null)
                            {
                                info.name = reader.ReadString();//实体名
                            }
                        }
                        if (reader.Name == "table")
                        {
                            if (reader.ReadToDescendant("key"))
                            {
                                string table = reader.GetAttribute("name");
                                if (table == "name")
                                {
                                    string value = reader.GetAttribute("value");
                                    textBox4.Text = value;
                                }
                                if (reader.ReadToNextSibling("key"))
                                {
                                    table = reader.GetAttribute("name");
                                    if (table == "name")
                                    {
                                        string value = reader.GetAttribute("value");
                                        info.tableName = value;//实体对应表名
                                    }
                                }
                            }
                        }
                        if (reader.Name == "ownProperty" || reader.Name == "linkProperty")
                        {
                            string link = reader.Name;
                            if (reader.ReadToDescendant("name"))
                            {
                                string name = reader.ReadString();
                                if (name == "parent")
                                {
                                    continue;
                                }
                                dict.Add(name, null);
                                dict2.Add(name, null);
                                dict3.Add(name, null);
                                dict4.Add(name, null);
                                dict5.Add(name, null);

                                if (reader.ReadToNextSibling("configured"))
                                {
                                    while (reader.Read())
                                    {
                                        string r = reader.Name;
                                        if (r == "mappingField")
                                        {
                                            if (reader.ReadToDescendant("key"))
                                            {
                                                string table = reader.GetAttribute("value");
                                                dict2[name] = table;
                                            }
                                        }
                                        else if (r == "relationship")
                                        {
                                            reader.ReadToDescendant("key");
                                            string package = reader.GetAttribute("value");
                                            reader.ReadToNextSibling("key");
                                            string fileName = reader.GetAttribute("value");
                                            //格式化对应关联关系文件路径
                                            string[] packings = package.Split('.');
                                            path = textBox1.Text;
                                            path += "\\metadata";
                                            foreach (string item in packings)
                                            {
                                                path += "\\" + item;
                                            }
                                            path += "\\" + fileName + ".relation";
                                            string tableName = getRelTableName(path);
                                            dict5[name] = getRelTablePath(path);
                                            dict3[name] = tableName;
                                            break;
                                        }
                                        else if (r == "dataType")
                                        {
                                            if (link == "ownProperty")
                                            {
                                                string dataType = reader.ReadElementString();
                                                dict4[name] = dataType;
                                                if (reader.ReadToNextSibling("metadataRef"))
                                                {
                                                    if (!reader.IsEmptyElement)
                                                    {
                                                        string enumName = reader.ReadElementString();
                                                        string[] enumNames = enumName.Split('.');
                                                        dict3[name] = enumNames[enumNames.Length - 1];
                                                        dict5[name] = getMetaDataFullPath(enumName, ".enum");
                                                    }
                                                }
                                            }
                                        }
                                        if (r == "linkProperty" || r == "ownProperty")
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (reader.Name == "rs")
                        {
                            if (n == 0)
                            {
                                if (reader.ReadToDescendant("lang"))
                                {
                                    do
                                    {
                                        string locale = reader.GetAttribute("locale");
                                        if (locale == "zh_CN")
                                        {
                                            columnName = reader.GetAttribute("value");
                                            info.alias = columnName;//实体 别名
                                            break;
                                        }
                                    } while (reader.ReadToNextSibling("lang"));

                                }
                                n++;
                                continue;
                            }
                            string keyStr = reader.GetAttribute("key");
                            int start = keyStr.IndexOf("ownProperty[");
                            //int start = keyStr.IndexOf("linkProperty[");
                            int start2 = keyStr.IndexOf("linkProperty[");
                            int end = keyStr.IndexOf("].alias");
                            if ((start > 0 || start2 > 0) && end > 0 && (start + 12 < keyStr.Length || start2 + 13 < keyStr.Length))
                            {
                                if (start > 0)
                                {
                                    column = keyStr.Substring(start + 12, end - start - 12);
                                }
                                else if (start2 > 0)
                                {
                                    column = keyStr.Substring(start2 + 13, end - start2 - 13);
                                }
                                if (column == "parent" || column == "parent1")
                                {
                                    continue;
                                }
                                if (reader.ReadToDescendant("lang"))
                                {
                                    do
                                    {
                                        keyStr = reader.GetAttribute("locale");
                                        if (keyStr == "zh_CN")
                                        {
                                            columnName = reader.GetAttribute("value");
                                            dict[column] = columnName;
                                            break;
                                        }
                                    } while (reader.ReadToNextSibling("lang"));

                                }
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
            reader.Close();

            foreach (var item in dict)
            {
                FieldInfo fieldInfo = new FieldInfo();
                fieldInfo.name = empToValue(item.Key);
                fieldInfo.alias = empToValue(item.Value);
                fieldInfo.tableName = empToValue(dict2[item.Key]);
                fieldInfo.relTable = empToValue(dict3[item.Key]);
                fieldInfo.dataType = empToValue(dict4[item.Key]);
                fieldInfo.fullPath = empToValue(dict5[item.Key]);
                info.Add(fieldInfo);
            }
            //解析基类实体
            info.baseEntity = baseEntityParse(baseEntity);

            TableDataLink tableNode = new TableDataLink();
            tableNode.entity = info;
            if (thisTableNode != null)
            {
                tableNode.upNode = thisTableNode;
            }
            thisTableNode = tableNode;

            fillTableEntity(info);

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

        }

        private EntityInfo baseEntityParse(string path)
        {
            XmlTextReader reader = null;
            if (!File.Exists(path))
            {
                reader = getJarMetaObject(path);
                if (reader == null || path == "")
                {
                    return null;
                }
            }
            else
            {
                reader = new XmlTextReader(path);
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();//字段名
            Dictionary<string, string> dict2 = new Dictionary<string, string>();//数据库表名
            Dictionary<string, string> dict3 = new Dictionary<string, string>();//关联关系表
            Dictionary<string, string> dict4 = new Dictionary<string, string>();//数据类型
            Dictionary<string, string> dict5 = new Dictionary<string, string>();//关联关系表-具体路径

            int n = 0;
            string baseEntity = "";

            EntityInfo info = new EntityInfo();
            try
            {


                while (reader.Read())
                {
                    string column = "";
                    string columnName = "";
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "baseEntity")
                        {
                            if (reader.ReadToDescendant("key"))
                            {
                                string pack = reader.GetAttribute("value");
                                if (reader.ReadToNextSibling("key"))
                                {
                                    string name = reader.GetAttribute("value");
                                    baseEntity = getMetaDataFullPath(pack + "." + name, ".entity");
                                }
                            }
                        }
                        else if (reader.Name == "name")
                        {
                            if (info.name == null)
                            {
                                info.name = reader.ReadString();//实体名
                            }
                        }
                        if (reader.Name == "table")
                        {
                            if (reader.ReadToDescendant("key"))
                            {
                                string table = reader.GetAttribute("name");
                                if (reader.ReadToNextSibling("key"))
                                {
                                    table = reader.GetAttribute("name");
                                    if (table == "name")
                                    {
                                        string value = reader.GetAttribute("value");
                                        info.tableName = value;//实体表名
                                    }
                                }
                            }
                        }
                        if (reader.Name == "ownProperty" || reader.Name == "linkProperty")
                        {
                            string link = reader.Name;
                            if (reader.ReadToDescendant("name"))
                            {
                                string name = reader.ReadString();
                                if (name == "parent")
                                {
                                    continue;
                                }
                                dict.Add(name, null);
                                dict2.Add(name, null);
                                dict3.Add(name, null);
                                dict4.Add(name, null);
                                dict5.Add(name, null);

                                if (reader.ReadToNextSibling("userDefined"))
                                {
                                    while (reader.Read())
                                    {
                                        string r = reader.Name;
                                        if (r == "mappingField")
                                        {
                                            if (reader.ReadToDescendant("key"))
                                            {
                                                string table = reader.GetAttribute("value");
                                                dict2[name] = table;
                                            }
                                        }
                                        else if (r == "relationship")
                                        {
                                            reader.ReadToDescendant("key");
                                            string package = reader.GetAttribute("value");
                                            reader.ReadToNextSibling("key");
                                            string fileName = reader.GetAttribute("value");
                                            //格式化对应关联关系文件路径
                                            string[] packings = package.Split('.');
                                            path = textBox1.Text;
                                            path += "\\metadata";
                                            foreach (string item in packings)
                                            {
                                                path += "\\" + item;
                                            }
                                            path += "\\" + fileName + ".relation";
                                            string tableName = getRelTableName(path);
                                            dict5[name] = getRelTablePath(path);
                                            dict3[name] = tableName;
                                            break;
                                        }
                                        else if (r == "dataType")
                                        {
                                            if (link == "ownProperty")
                                            {
                                                string dataType = reader.ReadElementString();
                                                dict4[name] = dataType;
                                                if (reader.ReadToNextSibling("metadataRef"))
                                                {
                                                    if (!reader.IsEmptyElement)
                                                    {
                                                        string enumName = reader.ReadElementString();
                                                        string[] enumNames = enumName.Split('.');
                                                        dict3[name] = enumNames[enumNames.Length - 1];
                                                        dict5[name] = getMetaDataFullPath(enumName, ".enum");
                                                    }
                                                }
                                            }
                                        }
                                        if (r == "linkProperty" || r == "ownProperty")
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (reader.Name == "rs")
                        {
                            if (n == 0)
                            {
                                if (reader.ReadToDescendant("lang"))
                                {
                                    do
                                    {
                                        string locale = reader.GetAttribute("locale");
                                        if (locale == "zh_CN")
                                        {
                                            columnName = reader.GetAttribute("value");
                                            info.alias = columnName;//实体别名
                                            break;
                                        }
                                    } while (reader.ReadToNextSibling("lang"));

                                }
                                n++;
                                continue;
                            }
                            string keyStr = reader.GetAttribute("key");
                            int start = keyStr.IndexOf("ownProperty[");
                            //int start = keyStr.IndexOf("linkProperty[");
                            int start2 = keyStr.IndexOf("linkProperty[");
                            int end = keyStr.IndexOf("].alias");
                            if ((start > 0 || start2 > 0) && end > 0 && (start + 12 < keyStr.Length || start2 + 13 < keyStr.Length))
                            {
                                if (start > 0)
                                {
                                    column = keyStr.Substring(start + 12, end - start - 12);
                                }
                                else if (start2 > 0)
                                {
                                    column = keyStr.Substring(start2 + 13, end - start2 - 13);
                                }
                                if (column == "parent" || column == "parent1")
                                {
                                    continue;
                                }
                                if (reader.ReadToDescendant("lang"))
                                {
                                    do
                                    {
                                        keyStr = reader.GetAttribute("locale");
                                        if (keyStr == "zh_CN")
                                        {
                                            columnName = reader.GetAttribute("value");
                                            //textBox2.AppendText(column + "\t" + columnName + "\n");
                                            //textBox3.AppendText(textBox4.Text + columnName + textBox5.Text + column + textBox6.Text + "\n");
                                            dict[column] = columnName;
                                            break;
                                        }
                                    } while (reader.ReadToNextSibling("lang"));

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            reader.Close();

            foreach (var item in dict)
            {
                FieldInfo fieldInfo = new FieldInfo();
                fieldInfo.name = empToValue(item.Key);
                fieldInfo.alias = empToValue(item.Value);
                fieldInfo.tableName = empToValue(dict2[item.Key]);
                fieldInfo.relTable = empToValue(dict3[item.Key]);
                fieldInfo.dataType = empToValue(dict4[item.Key]);
                fieldInfo.fullPath = empToValue(dict5[item.Key]);
                info.Add(fieldInfo);
            }
            //解析基类实体
            info.baseEntity = baseEntityParse(baseEntity);
            return info;
        }

        public ArrayList getIndexArray(String inputStr, String findStr)
        {
            ArrayList list = new ArrayList();
            int start = 0;
            while (start < inputStr.Length)
            {
                int index = inputStr.IndexOf(findStr, start);
                if (index >= 0)
                {
                    list.Add(index);
                    start = index + findStr.Length;
                }
                else
                {
                    break;
                }
            }
            return list;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            entiryParen(true);
        }

        /**
         * 初始化列表树
         */
        public void initTree()
        {
            treeView1.Nodes.Clear();
            //\\scm
            string path = textBox1.Text + "\\metadata\\com\\kingdee\\eas\\hse";
            if (Directory.Exists(path) == false)
            {
                MessageBox.Show("项目路径不正确!");
                return;
            }
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] dis = di.GetDirectories();
            List<Object> list = new List<Object>();
            foreach (DirectoryInfo dx in dis)
            {
                DirectoryInfo[] disx = dx.GetDirectories();
                foreach (DirectoryInfo dd in disx)
                {
                    string fileName = path + "\\" + dx.Name + "\\" + dd.Name + "\\" + dd.Name + ".package";
                    if (!File.Exists(fileName))
                    {
                        continue;
                    }
                    string rootName = getRootName(fileName, "package");
                    if (rootName == "null")
                    {
                        continue;
                    }
                    //添加父节点
                    TreeNode pnode = new TreeNode();
                    pnode.Text = rootName;
                    treeView1.Nodes.Add(pnode);
                    textBox3.AutoCompleteCustomSource.Add(rootName);
                    //添加子节点
                    FileInfo[] files = dd.GetFiles("*.bizunit");
                    foreach (FileInfo file in files)
                    {
                        string nodeName = getRootName(file.FullName, "bizUnit");
                        TreeNode node = new TreeNode();
                        node.Text = nodeName;
                        pnode.Nodes.Add(node);
                        textBox3.AutoCompleteCustomSource.Add(nodeName);

                        //添加实体节点
                        DirectoryInfo ds = new DirectoryInfo(file.DirectoryName + "\\app");
                        FileInfo[] nodefiles = ds.GetFiles("*.entity");
                        string name = file.Name.Substring(0, file.Name.IndexOf(".bizunit"));
                        foreach (FileInfo ff in nodefiles)
                        {
                            if (ff.Name.IndexOf(name) >= 0)
                            {
                                TreeNode xnode = new TreeNode();
                                xnode.Text = ff.Name;
                                xnode.Tag = ff.FullName;
                                node.Nodes.Add(xnode);
                                textBox3.AutoCompleteCustomSource.Add(ff.Name);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < treeView1.Nodes.Count; i++)
            {
                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                {
                    if (treeView1.Nodes[i].Nodes[j].Text.IndexOf("医疗销售结算") > -1
                        && treeView1.Nodes[i].Nodes[j].Text.Length == 7)
                    {
                        treeView1.Nodes[i].Nodes[j].Expand();
                        treeView1.SelectedNode = treeView1.Nodes[i].Nodes[j];
                        treeView1.Focus();
                        return;
                    }
                }

            }
            textBox3.Focus();
        }

        public string getRootName(string path, string typeName)
        {
            string name = "";
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "rs")
                    {
                        string keyStr = reader.GetAttribute("key");
                        int start = keyStr.IndexOf(typeName + "[");
                        int end = keyStr.IndexOf("].alias");
                        if (start >= 0 && end > 0)
                        {
                            if (reader.ReadToDescendant("lang"))
                            {
                                do
                                {
                                    keyStr = reader.GetAttribute("locale");
                                    if (keyStr == "zh_CN")
                                    {
                                        name = reader.GetAttribute("value");
                                        break;
                                    }
                                } while (reader.ReadToNextSibling("lang"));

                            }
                        }
                    }
                }
            }
            reader.Close();
            return name;
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            entiryParen(true);
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            //entiryParen();
        }

        private void entiryParen(bool falg)
        {
            if (falg)
            {
                //在左边树中可以找到
                TreeNode node = treeView1.SelectedNode;
                if (node == null || node.Nodes == null)
                {
                    dataGridView1.Rows.Clear();
                    textBox4.Text = "";
                    return;
                }
                if (node.Nodes.Count == 0 && node.Tag != null)
                {
                    string path = node.Tag.ToString();
                    xmlParse(path);
                }
                else
                {
                    dataGridView1.Rows.Clear();
                    textBox4.Text = "";
                }
            }
            else
            {
                //在左边树中找不到
                string outString = "";
                //GetValue("JarFileList", textBox3.Text, out outString, basePath);
                GetValue("metadata", textBox3.Text, out outString, entityPath);
                if (outString.Length > 0)
                {
                    string path = textBox1.Text + "\\metadata\\" + textBox3.Text;
                    xmlParse(path);
                }
                else
                {
                    GetValue("metadata", textBox3.Text, out outString, bizunitPath);
                    if (outString.Length > 0)
                    {
                        string entityName = outString;
                        GetValue("metadata", entityName, out outString, entityPath);
                        if (outString.Length > 0)
                        {
                            string path = textBox1.Text + "\\metadata\\" + entityName;
                            xmlParse(path);
                        }
                    }
                }
            }

        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string text = textBox3.Text;
                bool falg = false;
                for (int i = 0; i < treeView1.Nodes.Count; i++)
                {
                    if (treeView1.Nodes[i].Text.IndexOf(text) > -1
                            && treeView1.Nodes[i].Text.Length == text.Length)
                    {
                        treeView1.Nodes[i].Expand();
                        treeView1.SelectedNode = treeView1.Nodes[i];
                        treeView1.Focus();
                        falg = true;
                        break;
                    }
                    for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++)
                    {
                        if (treeView1.Nodes[i].Nodes[j].Text.IndexOf(text) > -1
                            && treeView1.Nodes[i].Nodes[j].Text.Length == text.Length)
                        {
                            treeView1.Nodes[i].Nodes[j].Expand();
                            treeView1.SelectedNode = treeView1.Nodes[i].Nodes[j];
                            treeView1.Focus();
                            falg = true;
                            break;
                        }
                        for (int k = 0; k < treeView1.Nodes[i].Nodes[j].Nodes.Count; k++)
                        {
                            if (treeView1.Nodes[i].Nodes[j].Nodes[k].Text.IndexOf(text) > -1
                            && treeView1.Nodes[i].Nodes[j].Nodes[k].Text.Length == text.Length)
                            {
                                treeView1.SelectedNode = treeView1.Nodes[i].Nodes[j].Nodes[k];
                                treeView1.Focus();
                                falg = true;
                                break;
                            }
                        }
                        if (falg)
                            break;
                    }
                    if (falg)
                        break;
                }
                entiryParen(falg);
            }
        }

        private void mainPanel_Activated(object sender, EventArgs e)
        {
            textBox3.Focus();
        }

        private string getRelTableName(string path)
        {
            string tableName = "";
            XmlTextReader reader = null;
            if (!File.Exists(path))
            {
                reader = getJarMetaObject(path);
                if (reader == null)
                {
                    return tableName;
                }
            }
            else
            {
                reader = new XmlTextReader(path);
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "supplierObject")
                    {
                        reader.ReadToDescendant("key");
                        string package = reader.GetAttribute("value");
                        reader.ReadToNextSibling("key");
                        string fileName = reader.GetAttribute("value");
                        //格式化对应关联关系文件路径
                        path = getMetaDataFullPath(package + "." + fileName, ".entity");
                        tableName = getEntityTableName(path);
                        if (tableName == "")
                        {
                            //在开发环境中未找到对应的entity,去jar包中寻找
                        }
                        return tableName;
                    }
                }
            }

            return tableName;
        }

        private string getEntityTableName(string path)
        {
            string tableName = "";
            XmlTextReader reader = null;
            if (!File.Exists(path))
            {
                reader = getJarMetaObject(path);
                if (reader == null)
                {
                    return tableName;
                }
            }
            else
            {
                reader = new XmlTextReader(path);
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "table")
                    {
                        reader.ReadToDescendant("key");
                        reader.ReadToNextSibling("key");
                        tableName = reader.GetAttribute("value");
                        break;
                    }
                }
            }
            reader.Close();

            return tableName;
        }

        private string getMetaDataFullPath(string domPath, string fileType)
        {
            string path = textBox1.Text;
            path += "\\metadata";
            string[] packings = domPath.Split('.');
            foreach (string item in packings)
            {
                path += "\\" + item;
            }
            path += fileType;
            return path;
        }

        private string getRelTablePath(string path)
        {
            string tableName = "";
            XmlTextReader reader = null;
            if (!File.Exists(path))
            {
                reader = getJarMetaObject(path);
                if (reader == null)
                {
                    return tableName;
                }
            }
            else
            {
                reader = new XmlTextReader(path);
            }
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "supplierObject")
                    {
                        reader.ReadToDescendant("key");
                        string package = reader.GetAttribute("value");
                        reader.ReadToNextSibling("key");
                        string fileName = reader.GetAttribute("value");
                        //格式化对应关联关系文件路径
                        tableName = getMetaDataFullPath(package + "." + fileName, ".entity");
                        return tableName;
                    }
                }
            }

            return tableName;
        }

        public EnumUI enumUI;

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.Rows[dataGridView1.CurrentRow.Index] == null)
            {
                return;
            }
            DataGridViewCellCollection cells = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells;
            object dataType = cells[4].Value;
            if (empToValue(dataType) == "" && cells[5].Value != null)
            {
                xmlParse(cells[5].Value.ToString());
            }
            else if ("Enum".Equals(dataType))
            {
                ArrayList list = enumParse(cells[5].Value.ToString());
                if (enumUI == null)
                {
                    enumUI = new EnumUI();
                }
                enumUI.ShowDialog(list);
            }
        }

        private ArrayList enumParse(string path)
        {
            ArrayList list = new ArrayList();
            XmlTextReader reader = null;
            if (!File.Exists(path))
            {
                reader = getJarMetaObject(path);
                if (reader == null)
                {
                    return list;
                }
            }
            else
            {
                reader = new XmlTextReader(path);
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();//
            Dictionary<string, string> dict2 = new Dictionary<string, string>();//
            int n = 0;
            int x = 0;
            string enumEng = "";
            string enumName = "";
            string enumtype = "";
            while (reader.Read())
            {
                string column = "";
                string value = "";
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "name" && x == 0)
                    {
                        enumEng = reader.ReadString();
                        x++;
                        continue;
                    }
                    if (reader.Name == "enumDataType")
                    {
                        enumtype = reader.ReadString();
                        continue;
                    }
                    if (reader.Name == "enumValue")
                    {
                        if (reader.ReadToDescendant("name"))
                        {
                            column = reader.ReadString();
                        }
                        if (reader.ReadToNextSibling("value"))
                        {
                            value = reader.ReadString();
                        }
                        dict.Add(column, value);
                        dict2.Add(column, value);
                    }
                    if (reader.Name == "rs")
                    {
                        if (n == 0)
                        {
                            reader.ReadToDescendant("lang");
                            if (!(reader.GetAttribute("locale") == "zh_CN"))
                            {
                                reader.ReadToNextSibling("lang");
                            }
                            //reader.ReadToNextSibling("lang");
                            enumName = reader.GetAttribute("value");
                            n++;
                            continue;
                        }
                        string key = reader.GetAttribute("key");
                        if (key.IndexOf("enumValues") > -1)
                        {
                            int start = key.IndexOf("enumValue[");
                            int end = key.IndexOf("].alias");
                            column = key.Substring(start + 10, end - start - 10);
                            reader.ReadToDescendant("lang");
                            if (!(reader.GetAttribute("locale") == "zh_CN"))
                            {
                                reader.ReadToNextSibling("lang");
                            }
                            dict2[column] = reader.GetAttribute("value");
                        }
                    }
                }
            }
            reader.Close();

            ArrayList listNode = new ArrayList();
            listNode.Add(enumEng);
            listNode.Add(enumName);
            listNode.Add(enumtype);
            list.Add(listNode);
            listNode = new ArrayList();
            listNode.Add("");
            listNode.Add("");
            listNode.Add("");
            list.Add(listNode);
            foreach (string key in dict.Keys)
            {
                listNode = new ArrayList();
                listNode.Add(key);
                listNode.Add(dict2[key]);
                listNode.Add(dict[key]);
                list.Add(listNode);
            }
            return list;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (thisTableNode.upNode != null)
            {
                dataGridView1.Rows.Clear();
                thisTableNode.upNode.downNode = thisTableNode;
                thisTableNode = thisTableNode.upNode;
                fillTableEntity(thisTableNode.entity);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (thisTableNode.downNode != null)
            {
                dataGridView1.Rows.Clear();
                thisTableNode = thisTableNode.downNode;
                fillTableEntity(thisTableNode.entity);
            }
        }

        #region 使用EntityInfo填充表格 fillTableEntity(info)
        private void fillTableEntity(EntityInfo info)
        {
            if (info == null)
            {
                return;
            }
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            //textBox4.Text = ((ArrayList)list[0])[2].ToString();

            //表字段
            int index = dataGridView1.Rows.Add();
            dataGridView1.Rows[index].Cells[0].Value = info.name;
            dataGridView1.Rows[index].Cells[1].Value = info.alias;
            dataGridView1.Rows[index].Cells[2].Value = info.tableName;
            dataGridView1.Rows.Add();
            foreach (FieldInfo fieldInfo in info.fields)
            {
                index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = fieldInfo.name;
                dataGridView1.Rows[index].Cells[1].Value = fieldInfo.alias;
                dataGridView1.Rows[index].Cells[2].Value = fieldInfo.tableName;
                dataGridView1.Rows[index].Cells[3].Value = fieldInfo.relTable;
                dataGridView1.Rows[index].Cells[4].Value = fieldInfo.dataType;
                dataGridView1.Rows[index].Cells[5].Value = fieldInfo.fullPath;
            }
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            if (info.baseEntity != null)
            {
                //递归添加
                dataGridView1.Rows.Add();
                fillTableEntity(info.baseEntity);
            }
        }
        #endregion

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                button4_Click(sender, e);
            }
            else if (e.KeyChar.ToString().ToUpper() == "Q")
            {
                Console.WriteLine("QQQQQQQQQQQQQQQ");
                if (dropDown != null)
                {
                    if (thisTableNode != null)
                    {
                        DataGridViewRow row = dataGridView1.Rows[0];
                        string mainTable = row.Cells[2].Value.ToString();
                        string alias = "main";
                        //查询部分
                        StringBuilder selectStr = new StringBuilder();
                        //子分录 或F7 表连接
                        StringBuilder entrySql = new StringBuilder();
                        DataGridViewSelectedCellCollection selectCells = dataGridView1.SelectedCells;
                        HashSet<int> rowIndexs = new HashSet<int>();
                        foreach (DataGridViewCell selectCell in selectCells)
                        {
                            rowIndexs.Add(selectCell.RowIndex);
                        }
                        foreach (int index in rowIndexs)
                        {
                            DataGridViewRow selectRow = dataGridView1.Rows[index];
                            if (selectRow.Index == 0)
                            {
                                continue;
                            }
                            var zorValue = selectRow.Cells[0].Value;
                            var twoValue = selectRow.Cells[2].Value;
                            var thrValue = selectRow.Cells[3].Value;
                            var forValue = selectRow.Cells[4].Value;
                            if (zorValue == null || twoValue == null || thrValue == null || forValue == null)
                            {
                                continue;
                            }
                            string columnAlias = zorValue.ToString();
                            if (twoValue.ToString() == "" && thrValue.ToString() == "")
                            {
                                continue;
                            }
                            if (twoValue.ToString() != "" && thrValue.ToString() != "" && forValue.ToString() == "")
                            {
                                //F7 字段 和表
                                string key = twoValue.ToString();
                                string f7TableName = thrValue.ToString();
                                entrySql.Append("left join ").Append(f7TableName).Append(" ");
                                entrySql.Append(columnAlias).Append(" on ").Append(columnAlias).Append(".fid");
                                entrySql.Append("=#1.").Append(key);
                                entrySql.Append(Environment.NewLine);
                            }
                            else if (twoValue.ToString() != "" && thrValue.ToString() != "" && forValue.ToString() != "")
                            {
                                //当前表字段--枚举
                                selectStr.Append("#1.").Append(twoValue.ToString()).Append(" ").Append(columnAlias).Append(",");
                                selectStr.Append(Environment.NewLine);
                            }
                            else if (twoValue.ToString() != "" && thrValue.ToString() == "")
                            {
                                //当前表字段
                                selectStr.Append("#1.").Append(twoValue.ToString()).Append(" ").Append(columnAlias).Append(",");
                                selectStr.Append(Environment.NewLine);
                            }

                            else if (twoValue.ToString() == "" && thrValue.ToString() != "")
                            {
                                //下级分录
                                string f7TableName = thrValue.ToString();
                                entrySql.Append("left join ").Append(f7TableName).Append(" ");
                                entrySql.Append(columnAlias).Append(" on ").Append(columnAlias).Append(".fparentid");
                                entrySql.Append("=#1.fid");
                                entrySql.Append(Environment.NewLine);
                            }
                        }
                        selectStr.Append("#1.fid ID").Append(Environment.NewLine); ;
                        //SQL主体
                        StringBuilder sql = new StringBuilder();
                        sql.Append("select").Append(Environment.NewLine); ;
                        //sql.Append("#1.*").Append(Environment.NewLine);
                        sql.Append(selectStr);
                        sql.Append("from #0 #1").Append(Environment.NewLine);
                        sql.Append(entrySql);
                        //替换
                        sql.Replace("#0", mainTable);
                        sql.Replace("#1", alias);
                        txtSql.Text = sql.ToString();
                    }
                    dropDown.Show(this, 880, 300);
                }
                //BulidSql bSql = new BulidSql();
                //bSql.StartPosition = FormStartPosition.Manual;
                //Point startPoint = MousePosition;
                //startPoint.X = startPoint.X + 10;
                //startPoint.Y = startPoint.Y + 10;
                //bSql.Location = startPoint;
                //bSql.ShowInTaskbar = false;
                //bSql.Show();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                WritePrivateProfileString("Information", "select", "1", basePath);
            }
            else
            {
                WritePrivateProfileString("Information", "select", "2", basePath);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                WritePrivateProfileString("Information", "select", "1", basePath);
            }
            else
            {
                WritePrivateProfileString("Information", "select", "2", basePath);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            label2.Visible = false;
            Thread thread1 = new Thread(new ParameterizedThreadStart(initJarMetaDice));
            thread1.Start();
        }

        private void initJarMetaDice(object str)
        {
            WritePrivateProfileString("Information", "isInitJar", "0", basePath);
            //string[] sources = { "\\basemetas\\bos", "\\basemetas\\eas" };
            List<string> sourceList = new List<string>();
            if (radioButton1.Checked)
            {
                sourceList.Add("\\basemetas\\bos");
                sourceList.Add("\\basemetas\\eas");
            }
            else if (radioButton2.Checked)
            {
                sourceList.Add("\\metas\\bos");
                sourceList.Add("\\metas\\eas");
                sourceList.Add("\\metas\\sp");
            }

            int one = 0;
            int two = 0;
            int ban = 100 / sourceList.Count;
            foreach (string source in sourceList)
            {
                string filePath = textBox1.Text + source;
                DirectoryInfo di = new DirectoryInfo(filePath);
                if (!di.Exists)
                {
                    MessageBox.Show("文件夹路径不正确!!!");
                    return;
                }
                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (file.FullName.IndexOf("\\metas\\sp") > -1 && file.FullName.IndexOf("\\metas\\sp\\sp_scm-metas.jar") < 0)
                    {
                        continue;
                    }
                    ZipInputStream zipStream = new ZipInputStream(File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));
                    ZipEntry entry = zipStream.GetNextEntry();
                    Dictionary<string, byte[]> dict = new Dictionary<string, byte[]>();
                    while (entry != null)
                    {
                        if (!entry.IsDirectory)
                        {
                            if (entry.Name.IndexOf(".entity") > -1
                                || entry.Name.IndexOf(".enum") > -1
                                || entry.Name.IndexOf(".relation") > -1
                                || entry.Name.IndexOf(".package") > -1
                                || entry.Name.IndexOf(".bizunit") > -1
                                //|| entry.Name.IndexOf(".table") > -1
                                )
                            {
                                string mateName = entry.Name.Substring(entry.Name.LastIndexOf('/') + 1, entry.Name.Length - entry.Name.LastIndexOf('/') - 1);
                                string matePath = "";
                                if (radioButton1.Checked)
                                {
                                    matePath = file.FullName.Replace(textBox1.Text + "\\basemetas\\", "");
                                }
                                else
                                {
                                    matePath = file.FullName.Replace(textBox1.Text + "\\metas\\", "");
                                }
                                //WritePrivateProfileString("JarFileList", mateName, matePath, basePath);
                                writeMetaDataToConfig(zipStream, entry, mateName, matePath);
                                wirteLog(DateTime.Now.ToString("yyyyMMdd-HHmm") + ":" + matePath + "\t" + entry.Name + "\n");
                            }
                        }
                        //获取下一个文件
                        entry = zipStream.GetNextEntry();
                    }
                    zipStream.Close();
                    if (progressBar1.InvokeRequired)
                    {
                        // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                        Action<string> actionDelegate = (x) => { progressBar1.Value = (int)(ban * one + ((double)two) / files.Length * ban); };
                        // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                        progressBar1.Invoke(actionDelegate, str);
                    }
                    two++;
                }
                two = 0;
                one++;
            }
            if (progressBar1.InvokeRequired)
            {
                // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                Action<string> actionDelegate = (x) => { progressBar1.Value = 100; };
                this.label2.Invoke(actionDelegate, str);
            }
            WritePrivateProfileString("Information", "isInitJar", "1", basePath);
            Action<string> actionDelegate2 = (x) =>
            {
                if (radioButton2.Checked)
                {
                    textBox3.AutoCompleteMode = AutoCompleteMode.None;
                    textBox3.AutoCompleteSource = AutoCompleteSource.None;
                    textBox3.AutoCompleteCustomSource.Clear();
                    List<string> jarMetas = ReadSingleSection("metadata", entityPath);
                    foreach (string jarMetaName in jarMetas)
                    {
                        textBox3.AutoCompleteCustomSource.Add(jarMetaName);
                    }
                    jarMetas = ReadSingleSection("metadata", bizunitPath);
                    foreach (string jarMetaName in jarMetas)
                    {
                        textBox3.AutoCompleteCustomSource.Add(jarMetaName);
                    }
                    textBox3.AutoCompleteMode = AutoCompleteMode.Suggest;
                    textBox3.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
            };
            textBox3.Invoke(actionDelegate2, str);
        }

        //从配置文件读取元数据
        private XmlTextReader getJarMetaObject(string path)
        {
            string matePath = "";
            string[] strs = path.Split('\\');
            string fileName = strs[strs.Length - 1];
            //GetValue("JarFileList", fileName, out matePath, basePath);
            if (path.IndexOf(".entity") > -1)
            {
                GetValue("metadata", fileName, out matePath, entityPath);
                //if (radioButton2.Checked)
                //{
                string entityName = "";
                GetValue("metadata", fileName, out entityName, entityToPackagePath);
                fileName = entityName;
                //}
            }
            else if (path.IndexOf(".relation") > -1)
            {
                GetValue("metadata", fileName, out matePath, relationPath);
            }
            else if (path.IndexOf(".enum") > -1)
            {
                GetValue("metadata", fileName, out matePath, enumPath);
            }
            else if (path.IndexOf(".bizunit") > -1)
            {
                GetValue("metadata", fileName, out matePath, bizunitPath);
            }
            else if (path.IndexOf(".package") > -1)
            {
                GetValue("metadata", fileName, out matePath, packagePath);
            }
            if (matePath != null && matePath.Length > 0)
            {
                string baseMetaPath = "\\basemetas\\";
                if (radioButton2.Checked)
                {
                    baseMetaPath = "\\metas\\";
                }
                string jarPath = textBox1.Text + baseMetaPath + matePath;
                if (File.Exists(jarPath))
                {
                    //ZipInputStream zipStream = null;
                    //if (!jarFile.ContainsKey(jarPath) || jarFile[jarPath] == null)
                    //{
                    //    Stream fileStream = File.Open(jarPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    //    zipStream = new ZipInputStream(fileStream);
                    //    byte[] bytes = new byte[fileStream.Length];
                    //    fileStream.Read(bytes, 0, bytes.Length);
                    //    fileStream.Position = 0;
                    //    jarFile[jarPath] = bytes;
                    //    //jarFileStream[jarPath] = zipStream;
                    //}
                    //else
                    //{
                    //    byte[] bytes = jarFile[jarPath];
                    //    Stream newStream = new MemoryStream(bytes);
                    //    BufferedStream stream = new BufferedStream(newStream);
                    //    zipStream = new ZipInputStream(stream);
                    //    //Console.WriteLine("*******************" + n);
                    //    n++;
                    //}

                    //ZipEntry entry = zipStream.GetNextEntry();
                    //while (entry != null)
                    //{
                    //    if (!entry.IsDirectory)
                    //    {
                    //        if (entry.Name.IndexOf("/" + fileName) > -1 || entry.Name.Equals(fileName))
                    //        {
                    //            byte[] data = new byte[zipStream.Length];
                    //            zipStream.Read(data, 0, data.Length);
                    //            BufferedStream stream = new BufferedStream(new MemoryStream(data));
                    //            return new XmlTextReader(stream);
                    //        }
                    //    }
                    //    //获取下一个文件
                    //    entry = zipStream.GetNextEntry();
                    //}
                    //zipStream.Close();

                    //--------------------------------------------
                    ZipInputStream zipStream = null;
                    if (!jarZipStream.ContainsKey(jarPath) || jarZipStream[jarPath] == null)
                    {
                        Stream fileStream = File.Open(jarPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        zipStream = new ZipInputStream(fileStream);

                        byte[] bytes = new byte[fileStream.Length];
                        fileStream.Read(bytes, 0, bytes.Length);
                        fileStream.Position = 0;
                        Stream newStream = new MemoryStream(bytes);
                        BufferedStream stream = new BufferedStream(newStream);
                        jarZipStream[jarPath] = stream;
                        //jarFileStream[jarPath] = zipStream;
                    }
                    else
                    {
                        BufferedStream stream = jarZipStream[jarPath];
                        stream.Position = 0;
                        zipStream = new ZipInputStream(stream);
                        //Console.WriteLine("*******************" + n);
                        n++;
                    }

                    ZipEntry entry = zipStream.GetNextEntry();
                    while (entry != null)
                    {
                        if (!entry.IsDirectory)
                        {
                            if (entry.Name.IndexOf("/" + fileName) > -1 || entry.Name.Equals(fileName))
                            {
                                byte[] data = new byte[zipStream.Length];
                                zipStream.Read(data, 0, data.Length);
                                BufferedStream stream = new BufferedStream(new MemoryStream(data));
                                return new XmlTextReader(stream);
                            }
                        }
                        //获取下一个文件
                        entry = zipStream.GetNextEntry();
                    }
                    zipStream.Close();

                    //------------------------------------------------------------------
                    //Ionic.Zip.ZipFile zipFile = null;
                    //if (!jarZipFile.ContainsKey(jarPath) || jarZipFile[jarPath] == null)
                    //{
                    //    zipFile = new Ionic.Zip.ZipFile(jarPath);
                    //    jarZipFile[jarPath] = zipFile;
                    //}
                    //else
                    //{
                    //    zipFile = jarZipFile[jarPath];
                    //}
                    //foreach (Ionic.Zip.ZipEntry entry in zipFile)
                    //{
                    //    if (!entry.IsDirectory)
                    //    {
                    //        if (entry.FileName.IndexOf("/" + fileName) > -1 || entry.FileName.Equals(fileName))
                    //        {
                    //            //byte[] data = new byte[entry.Size];
                    //            //Console.WriteLine(entry.ToString());
                    //            //Ionic.Zip.ZipInputStream
                    //            MemoryStream memory = new MemoryStream();
                    //            entry.Extract(memory);
                    //            BufferedStream stream = new BufferedStream(memory);
                    //            return new XmlTextReader(stream);
                    //        }
                    //    }
                    //}

                    //----------------------------------------------------
                    //Ionic.Zip.ZipInputStream zipStream = null;
                    //if (!jarFileStream.ContainsKey(jarPath) || jarFileStream[jarPath] == null)
                    //{
                    //    Stream fileStream = File.Open(jarPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    //    zipStream = new Ionic.Zip.ZipInputStream(fileStream);
                    //    jarFileStream[jarPath] = zipStream;
                    //}
                    //else
                    //{
                    //    zipStream = jarFileStream[jarPath];
                    //    zipStream.Position = 0;
                    //    Console.WriteLine("*************** : " + n);
                    //    n++;
                    //}

                    //Ionic.Zip.ZipEntry entry = zipStream.GetNextEntry();
                    //while (entry != null)
                    //{
                    //    if (!entry.IsDirectory)
                    //    {
                    //        if (entry.FileName.IndexOf("/" + fileName) > -1 || entry.FileName.Equals(fileName))
                    //        {
                    //            byte[] data = new byte[zipStream.Length];
                    //            zipStream.Read(data, 0, data.Length);
                    //            BufferedStream stream = new BufferedStream(new MemoryStream(data));
                    //            return new XmlTextReader(stream);
                    //        }
                    //    }
                    //    //获取下一个文件
                    //    entry = zipStream.GetNextEntry();
                    //}
                    //zipStream.Close();

                }
            }
            return null;
        }

        Dictionary<string, byte[]> jarFile = new Dictionary<string, byte[]>();
        Dictionary<string, Ionic.Zip.ZipFile> jarZipFile = new Dictionary<string, Ionic.Zip.ZipFile>();
        Dictionary<string, BufferedStream> jarZipStream = new Dictionary<string, BufferedStream>();
        Dictionary<string, Ionic.Zip.ZipInputStream> jarFileStream = new Dictionary<string, Ionic.Zip.ZipInputStream>();
        public int n = 1;

        public string empToValue(object o)
        {
            return o == null ? "" : o.ToString();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            //筛选
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            for (int i = 2; i < dataGridView1.Rows.Count - 1; i++)
            {

                DataGridViewRow row = dataGridView1.Rows[i];
                DataGridViewBand band = dataGridView1.Rows[i];
                if (textBox4.Text.Length > 0)
                {
                    string name = empToValue(row.Cells[0].Value).ToUpper();
                    string alias = empToValue(row.Cells[1].Value).ToUpper();
                    string selectText = textBox4.Text.ToUpper();
                    if (name.IndexOf(selectText) > -1 || alias.IndexOf(selectText) > -1)
                    {
                        //row.DefaultCellStyle.BackColor = Color.Yellow;
                        band.Visible = true;
                    }
                    else
                    {
                        //row.DefaultCellStyle.BackColor = Color.White;
                        band.Visible = false;
                    }
                }
                else
                {
                    //row.DefaultCellStyle.BackColor = Color.White;
                    band.Visible = true;
                }
            }
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        // 读取section
        public List<string> ReadSections(string iniFilename)
        {
            List<string> result = new List<string>();
            byte[] buf = new byte[65536];
            uint len = GetPrivateProfileString(null, null, null, buf, (uint)buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }
        // 读取指定区域Keys列表。
        public List<string> ReadSingleSection(string Section, string iniFilename)
        {
            List<string> result = new List<string>();
            byte[] buf = new byte[2097152];
            uint lenf = GetPrivateProfileString(Section, null, null, buf, (uint)buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < lenf; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }

        //将元数据写入 配置文件
        public void writeMetaDataToConfig(ZipInputStream zipStream, ZipEntry entry, string mateName, string matePath)
        {
            if (entry.Name.IndexOf(".entity") > -1)
            {
                WritePrivateProfileString("metadata", mateName, matePath, entityPath);

                WritePrivateProfileString("metadata", mateName, entry.Name, entityToPackagePath);
            }
            else if (entry.Name.IndexOf(".relation") > -1)
            {
                WritePrivateProfileString("metadata", mateName, matePath, relationPath);
            }
            else if (entry.Name.IndexOf(".enum") > -1)
            {
                WritePrivateProfileString("metadata", mateName, matePath, enumPath);
            }
            else if (entry.Name.IndexOf(".bizunit") > -1)
            {
                byte[] data = new byte[zipStream.Length];
                zipStream.Read(data, 0, data.Length);
                BufferedStream stream = new BufferedStream(new MemoryStream(data));
                string name = MetaDataUtil.getPackageOrBizunitName(new XmlTextReader(stream), "bizUnit");
                WritePrivateProfileString("metadata", name, mateName.Replace(".bizunit", ".entity"), bizunitPath);
            }
            else if (entry.Name.IndexOf(".package") > -1)
            {
                byte[] data = new byte[zipStream.Length];
                zipStream.Read(data, 0, data.Length);
                BufferedStream stream = new BufferedStream(new MemoryStream(data));
                string name = MetaDataUtil.getPackageOrBizunitName(new XmlTextReader(stream), "package");
                WritePrivateProfileString("metadata", name, entry.Name.Substring(0, entry.Name.LastIndexOf(".")), packagePath);
            }
        }

        public void wirteLog(string text)
        {
            using (StreamWriter output = File.AppendText(logPath))
            {
                output.WriteLine(text);
                output.Close();
            }
        }
    }
}
