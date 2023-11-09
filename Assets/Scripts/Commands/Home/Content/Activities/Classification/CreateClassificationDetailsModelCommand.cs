using CI.TaskParallel;
using Newtonsoft.Json;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class CreateClassificationDetailsModelCommand : ICommandWithParameters
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private ClassificationDetailsModel _model;

        public void Execute(ISignal signal)
        {
#if UNITY_WEBGL
            RunInBackgroundWebGL(signal);
#else
            RunInBackground(signal);
#endif
        }

        private void RunInBackground(ISignal signal)
        {
            var errorMessage = string.Empty;
            ClassificationDetailsModel.ClassificationDetailsModelStruct classificationDetailsModelStruct = null;
            UnityTask.Run(() =>
            {
                var parameter = (CreateClassificationDetailsModelCommandSignal) signal;
                var classificationDetailsResponse = JsonConvert.DeserializeObject<Classification.ClassificationResponse>(parameter.ClassificationDetailsResponse);

                if (classificationDetailsResponse.Success)
                {
                    var classification = classificationDetailsResponse.Classification;

                    var classificationDetailsStruct = new ClassificationDetailsModel.ClassificationDetailsModelStruct
                    {
                        ClassificationId = classification.ClassificationId,
                        ClassificationName = classification.ClassificationName,
                        Properties = classification.Properties,
                        ClassificationLocal = classification.ClassificationLocal
                    };

                    return classificationDetailsStruct;
                }

                errorMessage = classificationDetailsResponse.ErrorMessage;
                return null;
                
            }).ContinueOnUIThread(task =>
            {
                if (task.Result != null)
                {
                    _model.Update(task.Result);
                }
                else
                {
                    _signal.Fire(new PopupOverlaySignal(false));
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("Couldn't create classification details: " + errorMessage));
                }
            });
        }
        
        private void RunInBackgroundWebGL(ISignal signal)
        {
            var errorMessage = string.Empty;
            ClassificationDetailsModel.ClassificationDetailsModelStruct classificationDetailsModelStruct = null;

                var parameter = (CreateClassificationDetailsModelCommandSignal) signal;
                var classificationDetailsResponse = JsonConvert.DeserializeObject<Classification.ClassificationResponse>(parameter.ClassificationDetailsResponse);

                if (classificationDetailsResponse.Success)
                {
                    var classification = classificationDetailsResponse.Classification;

                    var classificationDetailsStruct = new ClassificationDetailsModel.ClassificationDetailsModelStruct
                    {
                        ClassificationId = classification.ClassificationId,
                        ClassificationName = classification.ClassificationName,
                        Properties = classification.Properties,
                        ClassificationLocal = classification.ClassificationLocal
                    };


                    classificationDetailsModelStruct = classificationDetailsStruct;
                }

                errorMessage = classificationDetailsResponse.ErrorMessage;


                if (classificationDetailsModelStruct != null)
                {
                    _model.Update(classificationDetailsModelStruct);
                }
                else
                {
                    _signal.Fire(new PopupOverlaySignal(false));
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("CreateClassificationDetailsModelCommand server response | " + errorMessage));
                }
        }
    }
}