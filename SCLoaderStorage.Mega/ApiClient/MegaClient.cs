﻿using CG.Web.MegaApiClient;
using SCLoaderShared.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCLoaderStorage.Mega.ApiClient
{
    class MegaClient
    {

        private MegaApiClient apiClient;


        public MegaClient(string email, string password)
        {

            this.apiClient = new MegaApiClient();

            try
            {
                apiClient.Login(email, password);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to log into Mega API with provided email and password.", ex);
            }

        }


        public INode GetOrAddDirectoryNode(string path)
        {

            // Fix possible directory separators in path to use the system separator
            var fixedPath = PathHelpers.FixPathSeparators(path, false);

            // Now use the system separator to get a the list of node names
            var nodeNames = fixedPath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            // Get the node object from API
            var allNodes = this.apiClient.GetNodes();
            var rootNode = allNodes.Where(n => n.Type == NodeType.Root).FirstOrDefault();

            INode currentNode = rootNode;

            IEnumerable<INode> currentChildNodes;
            INode nextNode;

            foreach (var nodeName in nodeNames)
            {

                currentChildNodes = this.apiClient.GetNodes(currentNode);

                nextNode = currentChildNodes
                    .Where(n => n.Type == NodeType.Directory && n.Name.Equals(nodeName, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                if (nextNode == null)
                {
                    nextNode = this.apiClient.CreateFolder(nodeName, currentNode);
                }

                currentNode = nextNode;

            }

            return currentNode;

        }


        public bool FileExists(INode directoryNode, string fileName)
        {

            return (GetFileNode(directoryNode, fileName) != null);

        }


        public string GetFileContent(INode directoryNode, string fileName)
        {

            var content = "";

            var fileNode = GetFileNode(directoryNode, fileName);
            if (fileNode != null)
            {
                var fileUri = this.apiClient.GetDownloadLink(fileNode);

                using (Stream fileStream = this.apiClient.Download(fileUri))
                {
                    using (var reader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        content = reader.ReadToEnd();
                    }
                }

            }

            return content;

        }


        public void SaveFileContent(INode directoryNode, string fileName, string content, bool overwriteExistingFile)
        {

            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            using (var stream = new MemoryStream(byteArray))
            {
                SaveFileStream(directoryNode, fileName, stream, overwriteExistingFile);
            }

        }

        public void SaveFileStream(INode directoryNode, string fileName, Stream stream, bool overwriteExistingFile)
        {

            /*
             * Note:
             * Files cannot be overwritten or updated on Mega.
             * We have to delete the old file after adding a new.
             */

            // Get the possible existing file node
            INode lockFileNode = null;
            if (overwriteExistingFile)
            {
                lockFileNode = GetFileNode(directoryNode, fileName);
            }

            // Add the new file
            this.apiClient.Upload(stream, fileName, directoryNode);

            // Remove the old file
            if (lockFileNode != null)
            {
                DeleteFile(lockFileNode, false);
            }

        }


        public void DeleteFile(INode directoryNode, string fileName, bool moveToTrash)
        {

            var fileNode = GetFileNode(directoryNode, fileName);
            if (fileNode != null)
            {
                DeleteFile(fileNode, moveToTrash);
            }

        }


        private void DeleteFile(INode fileNode, bool moveToTrash)
        {

            this.apiClient.Delete(fileNode, moveToTrash);

        }


        private INode GetFileNode(INode directoryNode, string fileName)
        {

            var childNodes = this.apiClient.GetNodes(directoryNode);
            return childNodes
                .Where(n => n.Type == NodeType.File && n.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(n => n.LastModificationDate)
                .FirstOrDefault();

        }


    }
}
