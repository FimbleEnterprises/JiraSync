using CrmToJira;
using CrmToJira.Objects;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace JIRA_API_Proxy.Objects {
	public class Note {

		public Note() {

		}

		/// <summary>
		/// Creates a note, downloads the attachment from Jira and converts it to base64
		/// </summary>
		/// <param name="jiraAttachment">A Jira attachment</param>
		/// <param name="crmIssueid">The crm issue id to create the note for</param>
		public Note(Attachment jiraAttachment, string crmIssueid, JiraCredentials Credentials) {
			this.subject = "Attachment";
			string base64 = DownloadAttachment(jiraAttachment.content, Credentials);
			this.documentbody = base64;
			this.isdocument = true;
			this.mimetype = jiraAttachment.mimeType;
			this.overriddencreatedon = jiraAttachment.created;
			this.modifiedon = jiraAttachment.created;
			this.filename = jiraAttachment.filename;
			this.filesize = jiraAttachment.size;
			this.objecttypecode = "msus_jiraissue";
			this._objectid_value = crmIssueid;
			this.notetext = "\n(" + jiraAttachment.author.displayName + ")";
		}

		public Note(Entity entity) {

			if (entity.Attributes.Contains("subject")) {
				this.subject = entity.Attributes["subject"].ToString();
			}
			if (entity.Attributes.Contains("documentbody")) {
				this.documentbody = entity.Attributes["documentbody"].ToString();
			}
			if (entity.Attributes.Contains("isdocument")) {
				this.isdocument = (bool) entity.Attributes["isdocument"];
			}
			if (entity.Attributes.Contains("mimetype")) {
				this.mimetype = entity.Attributes["mimetype"].ToString();
			}
			if (entity.Attributes.Contains("overriddencreatedon")) {
				this.overriddencreatedon = (DateTime) entity.Attributes["overriddencreatedon"];
			}
			if (entity.Attributes.Contains("modifiedon")) {
				this.modifiedon = (DateTime) entity.Attributes["modifiedon"];
			}
			if (entity.Attributes.Contains("filename")) {
				this.filename = entity.Attributes["filename"].ToString();
			}
			if (entity.Attributes.Contains("filesize")) {
				this.filesize = (int) entity.Attributes["filesize"];
			}
			if (entity.Attributes.Contains("objecttypecode")) {
				this.objecttypecode = entity.Attributes["objecttypecode"].ToString();
			}
			if (entity.Attributes.Contains("objectid")) {
				this.objectid = entity.Attributes["objectid"].ToString();
				this._objectid_value = entity.Attributes["objectid"].ToString();
			}
			if (entity.Attributes.Contains("notetext")) {
				this.notetext = entity.Attributes["notetext"].ToString();
			}
			if (entity.Attributes.Contains("annotationid")) {
				this.annotationid = entity.Attributes["annotationid"].ToString();
			}
		}

		public string etag { get; set; }
		public string documentbody { get; set; }
		public string documentbody_binary { get; set; }
		public string filename { get; set; }
		public string notetext { get; set; }
		public string mimetype { get; set; }
		public string _createdby_valueFormattedValue { get; set; }
		public string _createdby_value { get; set; }
		public string createdonFormattedValue { get; set; }
		public DateTime overriddencreatedon { get; set; }
		public DateTime createdon { get; set; }
		public DateTime modifiedon { get; set; }
		public string _objectid_value { get; set; }
		public string objectid { get; set; }
		public string subject { get; set; }
		public string isdocumentFormattedValue { get; set; }
		public bool isdocument { get; set; }
		public string objecttypecodeFormattedValue { get; set; }
		public string objecttypecode { get; set; }
		public string annotationid { get; set; }
		public string filesizeFormattedValue { get; set; }
		public int filesize { get; set; }

		public string DownloadAttachment(string url, JiraCredentials credentials) {
			try {

				// Construct the request
				HttpWebRequest newRequest = WebRequest.Create(url) as HttpWebRequest;
				newRequest.ContentType = "application/json";
				newRequest.Method = "GET";
				string base64Credentials = credentials.ToEncoded();
				newRequest.Headers.Add("Authorization", "Basic " + base64Credentials);
				newRequest.Accept = "application/json";

				// Make the request and get the response
				string path = HttpContext.Current.Server.MapPath("~/files");
				HttpWebResponse response = newRequest.GetResponse() as HttpWebResponse;
				// FileStream stream = new FileStream(path + "testfile.jpg", FileMode.Create);
				Stream inputstream = response.GetResponseStream();

				MemoryStream ms = new MemoryStream();
				inputstream.CopyTo(ms);
				byte[] bytes = ms.ToArray();
				string base64 = Convert.ToBase64String(bytes);

				// Clean up and return result
				newRequest = null;
				response = null;
				return base64;

			} catch (Exception e) {
				return null;
			}
		}
	}

	public class Notes {
		public string context { get; set; }
		public List<Note> value { get; set; }

		public EntityCollection ToEntities() {
			EntityCollection collection = new EntityCollection();
			foreach (Note note in this.value) {
				Entity entity = new Entity("annotation");
				entity.Attributes["subject"] = note.subject;
				entity.Attributes["notetext"] = note.notetext;
				entity.Attributes["overriddencreatedon"] = note.overriddencreatedon;
				entity.Attributes["modifiedon"] = note.overriddencreatedon;
				entity.Attributes["objecttypecode"] = "msus_jiraissue";
				EntityReference regarding = new EntityReference("msus_jiraissue", new Guid(note._objectid_value));
				entity.Attributes["objectid"] = regarding;
				if (note.isdocument) {
					entity.Attributes["filename"] = note.filename;
					entity.Attributes["documentbody"] = note.documentbody;
					entity.Attributes["mimetype"] = note.mimetype;
					entity.Attributes["filesize"] = note.filesize;
					entity.Attributes["isdocument"] = note.isdocument;
				}
				collection.Entities.Add(entity);
			}
			return collection;
		}
	}
}