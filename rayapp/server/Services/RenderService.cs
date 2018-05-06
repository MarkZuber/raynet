using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace rayapp
{
  public interface IRenderService
  {
    RenderDataModel GetRenderDataModel();
  }

  public class RenderDataModel
   {
      public string RenderName {get; set;}
      public string ImageRelativeUrl {get; set;}
      public bool IsRendering {get; set;}
      public int PercentComplete {get; set;}
   }

   public class RenderService : IRenderService
   {
      private RenderDataModel _renderDataModel;

      public RenderService()
      {
         _renderDataModel = new RenderDataModel();
      }

      public RenderDataModel GetRenderDataModel() {
        return _renderDataModel;
      }
   }
}
