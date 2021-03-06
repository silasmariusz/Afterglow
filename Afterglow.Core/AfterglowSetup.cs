﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Afterglow.Core.Plugins;
using Afterglow.Core.Load;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Afterglow.Core
{
    /// <summary>
    /// Contains the setup required to run Afterglow
    /// This is the root node in the XML saved document
    /// </summary>
    [DataContract]
    public class AfterglowSetup : BaseModel
    {

        /// <summary>
        /// All of the configured plugins exist here and then are referenced in each Profile
        /// </summary>
        /// <remarks>
        /// Loaded first by the XML Deserilization
        /// </remarks>
        #region Configured Plugins

        /// <summary>
        /// A list of configured Capture Plugins
        /// </summary>
        [DataMember]
        public SerializableInterfaceList<ICapturePlugin> ConfiguredCapturePlugins
        {
            get { return Get(() => ConfiguredCapturePlugins, new SerializableInterfaceList<ICapturePlugin>()); }
            set { Set(() => ConfiguredCapturePlugins, value); }
        }
        /// <summary>
        /// A list of configured Colour Extraction Plugins
        /// </summary>
        [DataMember]
        public SerializableInterfaceList<IColourExtractionPlugin> ConfiguredColourExtractionPlugins
        {
            get { return Get(() => ConfiguredColourExtractionPlugins, new SerializableInterfaceList<IColourExtractionPlugin>()); }
            set { Set(() => ConfiguredColourExtractionPlugins, value); }
        }
        /// <summary>
        /// A list of configured Light Setup Plugins
        /// </summary>
        [DataMember]
        public SerializableInterfaceList<ILightSetupPlugin> ConfiguredLightSetupPlugins
        {
            get { return Get(() => ConfiguredLightSetupPlugins, new SerializableInterfaceList<ILightSetupPlugin>()); }
            set { Set(() => ConfiguredLightSetupPlugins, value); }
        }
        /// <summary>
        /// A list of configured Post Process Plugins
        /// </summary>
        [DataMember]
        public SerializableInterfaceList<IPostProcessPlugin> ConfiguredPostProcessPlugins
        {
            get { return Get(() => ConfiguredPostProcessPlugins, new SerializableInterfaceList<IPostProcessPlugin>()); }
            set { Set(() => ConfiguredPostProcessPlugins, value); }
        }
        /// <summary>
        /// A list of configured Output Plugins
        /// </summary>
        [DataMember]
        public SerializableInterfaceList<IOutputPlugin> ConfiguredOutputPlugins
        {
            get { return Get(() => ConfiguredOutputPlugins, new SerializableInterfaceList<IOutputPlugin>()); }
            set { Set(() => ConfiguredOutputPlugins, value); }
        }
        #endregion

        //[XmlIgnore]
        //[DataMember]
        //public List<PluginCategory> PluginCategories
        //{
        //    get
        //    {
        //        var result = new List<PluginCategory>();

        //        PluginCategory lightSetupPlugins = new PluginCategory();
        //        lightSetupPlugins.Name = "Light Setup Plugins";
        //        lightSetupPlugins.InstalledPlugins =
        //            (from available in this.AvailableLightSetupPlugins
        //             select new InstalledPlugin() { Name = available.FullName, PluginType = available }
        //            ).ToList();
        //        result.Add(lightSetupPlugins);


        //        PluginCategory capturePlugins = new PluginCategory();
        //        capturePlugins.Name = "Capture Plugins";
        //        capturePlugins.InstalledPlugins =
        //            (from available in this.AvailableCapturePlugins
        //             select new InstalledPlugin() { Name = available.FullName, PluginType = available }
        //            ).ToList();
        //        result.Add(capturePlugins);


        //        PluginCategory colourExtractionPlugins = new PluginCategory();
        //        colourExtractionPlugins.Name = "Colour Extraction Plugins";
        //        colourExtractionPlugins.InstalledPlugins =
        //            (from available in this.AvailableColourExtractionPlugins
        //             select new InstalledPlugin() { Name = available.FullName, PluginType = available }
        //            ).ToList();
        //        result.Add(colourExtractionPlugins);


        //        PluginCategory postProcessPlugins = new PluginCategory();
        //        postProcessPlugins.Name = "Post Process Plugins";
        //        postProcessPlugins.InstalledPlugins =
        //            (from available in this.AvailablePostProcessPlugins
        //             select new InstalledPlugin() { Name = available.FullName, PluginType = available }
        //            ).ToList();
        //        result.Add(postProcessPlugins);


        //        PluginCategory outputPlugins = new PluginCategory();
        //        outputPlugins.Name = "Output Plugins";
        //        outputPlugins.InstalledPlugins =
        //            (from available in this.AvailableOutputPlugins
        //             select new InstalledPlugin() { Name = available.FullName, PluginType = available }
        //            ).ToList();
        //        result.Add(outputPlugins);
                
        //        return result;
        //    }
        //}

        //public class PluginCategory
        //{
        //    public string Name { get; set; }

        //    public List<InstalledPlugin> InstalledPlugins { get; set; }
        //}

        //public class InstalledPlugin
        //{
        //    public Type PluginType { get; set; }
        //    public string Name { get; set; }

        //}

        /// <summary>
        /// A Generic function to aid in the creation of Profile and IPlugin identifers
        /// </summary>
        /// <typeparam name="T">Accepted types are Profile, ICapturePlugin, IColourExtractionPlugin, ILightSetupPlugin, IPostProcessPlugin, IOutputPlugin</typeparam>
        /// <returns>Integer Id for a new object of the given type</returns>
        public int GetNewId<T>()
        {
            int result = 0;

            //Each query gets the last/largest Id used
            if (typeof(T) == typeof(Profile))
            {
                if (this.Profiles.Any())
                    result = this.Profiles.Max(profile => profile.Id);
            }
            else if (typeof(T) == typeof(ICapturePlugin))
            {
                if (this.ConfiguredCapturePlugins.Any())
                    result = this.ConfiguredCapturePlugins.Max(plugin => plugin.Id);
            }
            else if (typeof(T) == typeof(IColourExtractionPlugin))
            {
                if (this.ConfiguredColourExtractionPlugins.Any())
                    result = this.ConfiguredColourExtractionPlugins.Max(plugin => plugin.Id);
            }
            else if (typeof(T) == typeof(ILightSetupPlugin))
            {
                if (this.ConfiguredLightSetupPlugins.Any())
                    result = this.ConfiguredLightSetupPlugins.Max(plugin => plugin.Id);
            }
            else if (typeof(T) == typeof(IPostProcessPlugin))
            {
                if (this.ConfiguredPostProcessPlugins.Any())
                    result = this.ConfiguredPostProcessPlugins.Max(plugin => plugin.Id);
            }
            else if (typeof(T) == typeof(IOutputPlugin))
            {
                if (this.ConfiguredOutputPlugins.Any())
                    result = this.ConfiguredOutputPlugins.Max(plugin => plugin.Id);
            }
            return result++;
        }

        /// <summary>
        /// A list of all Profiles
        /// </summary>
        /// <remarks>
        /// Loaded Second by the XML Deserilization, to re-using Configured Afterglow Plugins objects to ensure referential integrity
        /// </remarks>
        [DataMember]
        public List<Profile> Profiles
        {
            get { return Get(() => Profiles, new List<Profile>()); }
            set { Set(() => Profiles, value); }
        }

        /// <summary>
        /// Adds a new profile
        /// </summary>
        /// <returns>New Profile</returns>
        public Profile AddNewProfile()
        {
            Profile newProfile = new Profile();
            newProfile.Id = GetNewId<Profile>();
            newProfile.LightSetupPlugins = DefaultLightSetupPlugins();
            newProfile.CapturePlugins = DefaultCapturePlugins();
            newProfile.ColourExtractionPlugins = DefaultColourExtractionPlugins();
            newProfile.PostProcessPlugins = DefaultPostProcessPlugins();
            newProfile.OutputPlugins = DefaultOutputPlugins();
            this.Profiles.Add(newProfile);

            return newProfile;
        }

        /// <summary>
        /// The Current Profile Id
        /// </summary>
        [DataMember]
        [Required]
        public int CurrentProfileId
        {
            get { return Get(() => CurrentProfileId, 1); }
            set { Set(() => CurrentProfileId, value); }
        }
        
        /// <summary>
        /// Gets the Type of Plugins, used when creating a new configured plugin
        /// </summary>
        /// <remarks>
        /// Not Loaded as they are loaded from the file system and the project
        /// </remarks>
        #region Available Plugin Types
        /// <summary>
        /// Available Light Setup Plugin Types
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public Type[] AvailableLightSetupPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<ILightSetupPlugin>(); }
        }
        /// <summary>
        /// Available Capture Plugin Types
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public Type[] AvailableCapturePlugins
        {
            get { return PluginLoader.Loader.GetPlugins<ICapturePlugin>(); }
        }
        /// <summary>
        /// Available Colour Extraction Plugin Types
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public Type[] AvailableColourExtractionPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<IColourExtractionPlugin>(); }
        }
        /// <summary>
        /// Available Post Process Plugin Types
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public Type[] AvailablePostProcessPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<IPostProcessPlugin>(); }
        }
        /// <summary>
        /// Available Available Output Plugin Types
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public Type[] AvailableOutputPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<IOutputPlugin>(); }
        }
        #endregion

        /// <summary>
        /// Gets the default plugins used when a new plugin is created
        /// </summary>
        #region Default Plugins
        /// <summary>
        /// Trys to get a configured Light Setup Plugin (no specific ordering),
        /// failing that it will attempt to create a new Light Setup Plugin from the available types (no specific ordering)
        /// </summary>
        /// <returns>A list with one Light Setup Plugin</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SerializableInterfaceList<ILightSetupPlugin> DefaultLightSetupPlugins()
        {
            SerializableInterfaceList<ILightSetupPlugin> lightSetupPlugins = new SerializableInterfaceList<ILightSetupPlugin>();
            if (this.ConfiguredLightSetupPlugins.Any())
            {
                lightSetupPlugins.Add(this.ConfiguredLightSetupPlugins.FirstOrDefault());
            }
            else
            {
                Type type = AvailableLightSetupPlugins.FirstOrDefault();
                if (type == null)
                {
                    //TODO log error
                    throw new ArgumentNullException("No ILightSetupPlugin's have been loaded, please check the install and try again");
                }
                else
                {
                    lightSetupPlugins.Add(Activator.CreateInstance(type) as ILightSetupPlugin);
                }
            }

            return lightSetupPlugins;
        }

        /// <summary>
        /// Trys to get a configured Capture Setup Plugin (no specific ordering),
        /// failing that it will attempt to create a new Capture Plugin from the available types (trys CopyScreenCapture first then, no specific ordering)
        /// </summary>
        /// <returns>A list with one Capture Plugin</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SerializableInterfaceList<ICapturePlugin> DefaultCapturePlugins()
        {
            SerializableInterfaceList<ICapturePlugin> capturePlugins = new SerializableInterfaceList<ICapturePlugin>();
            if (this.ConfiguredCapturePlugins != null && this.ConfiguredCapturePlugins.Any())
            {
                capturePlugins.Add(this.ConfiguredCapturePlugins.FirstOrDefault());
            }
            else
            {
                Type type = AvailableCapturePlugins.Where(a => a.Name == "CopyScreenCapture").FirstOrDefault();
                if (type == null)
                {
                    type = AvailableCapturePlugins.FirstOrDefault();
                }

                if (type == null)
                {
                    //TODO log error
                    throw new ArgumentNullException("No ICapturePlugin's have been loaded, please check the install and try again");
                }
                else
                {
                    capturePlugins.Add(Activator.CreateInstance(type) as ICapturePlugin);
                }
            }
            return capturePlugins;
        }

        /// <summary>
        /// Trys to get a configured Colour Extraction Plugin (no specific ordering),
        /// failing that it will attempt to create a new Colour Extraction Plugin from the available types (no specific ordering)
        /// </summary>
        /// <returns>A list with one Colour Extraction Plugin </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SerializableInterfaceList<IColourExtractionPlugin> DefaultColourExtractionPlugins()
        {
            SerializableInterfaceList<IColourExtractionPlugin> colourExtractionPlugins = new SerializableInterfaceList<IColourExtractionPlugin>();
            if (this.ConfiguredColourExtractionPlugins.Any())
            {
                colourExtractionPlugins.Add(this.ConfiguredColourExtractionPlugins.FirstOrDefault());
            }
            else
            {
                Type type = AvailableColourExtractionPlugins.FirstOrDefault();
                if (type == null)
                {
                    //TODO log error
                    throw new ArgumentNullException("No IColourExtractionPlugin's have been loaded, please check the install and try again");
                }
                else
                {
                    colourExtractionPlugins.Add(Activator.CreateInstance(type) as IColourExtractionPlugin);
                }
            }
            return colourExtractionPlugins;
        }

        /// <summary>
        /// Returns an empty list as there are no default Post Process Plugins
        /// </summary>
        /// <returns>An empty list object</returns>
        public SerializableInterfaceList<IPostProcessPlugin> DefaultPostProcessPlugins()
        {
            return new SerializableInterfaceList<IPostProcessPlugin>();
        }

        /// <summary>
        /// Trys to get a configured Output Plugin (no specific ordering),
        /// failing that it will attempt to create a new Output Plugin from the available types (no specific ordering)
        /// </summary>
        /// <returns>A list with one Output Plugin</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SerializableInterfaceList<IOutputPlugin> DefaultOutputPlugins()
        {
            SerializableInterfaceList<IOutputPlugin> outputPlugins = new SerializableInterfaceList<IOutputPlugin>();
            if (this.ConfiguredOutputPlugins.Any())
            {
                outputPlugins.Add(this.ConfiguredOutputPlugins.FirstOrDefault());
            }
            else
            {
                Type type = AvailableOutputPlugins.FirstOrDefault();
                if (type == null)
                {
                    //TODO log error
                    throw new ArgumentNullException("No IOutputPlugin's have been loaded, please check the install and try again");
                }
                else
                {
                    outputPlugins.Add(Activator.CreateInstance(type) as IOutputPlugin);
                }
            }
            return outputPlugins;
        }
        #endregion


        ///<summary>
        /// General settings applied to the Afterglow runtime
        ///</summary>
        #region General Settings

        /// <summary>
        /// Gets and Sets the Port for the web interface, default is 8080
        /// </summary>
        [DataMember]
        [Required]
        public int Port
        {
            get { return Get(() => Port, 8080); }
            set { Set(() => Port, value); }
        }
        /// <summary>
        /// Gets and Sets the UserName for the web interface, default is Afterglow
        /// </summary>
        [DataMember]
        [Required]
        public string UserName
        {
            get { return Get(() => UserName, "Afterglow"); }
            set { Set(() => UserName, value); }
        }
        /// <summary>
        /// Gets and Sets the Password for the web interface
        /// </summary>
        [DataMember]
        public string Password
        {
            get { return Get(() => Password); }
            set { Set(() => Password, value); }
        }
        #endregion

        /// <summary>
        /// When this object has been deserialized this will get called and set sub object settings
        /// </summary>
        internal void OnDeserialized()
        {
            //Set the parents
            foreach (Profile profile in this.Profiles)
            {
                profile.Setup = this;
                profile.OnDeserialized();
            }
        }
    }
}
