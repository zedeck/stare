using BackendAPI.Constants;
using BackendAPI.Database;
using BackendAPI.Global;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using BackendAPI.Services;
using Common.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BackendAPI.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    //[MIEAuthorize]
    public class QuestionnaireController : ControllerBase
    {
        private readonly IQuestionnaire _qservice;


        public QuestionnaireController(IQuestionnaire qservice)
        {
            _qservice = qservice;

        }

        [MIEAuthorize]
        [HttpGet]
        public async Task<ActionResult<List<AnswerType>>> GetAllAnswerTypes()
        {
            return Ok(await _qservice.GetAllAnswerTypes());
        }

        [MIEAuthorize]
        [HttpPost]
        public ActionResult<List<LQuestionsModel>> CreateAndSendReferenceForm(LCreateFormRequestModel LCFRequestModel)
        {
            return Ok(_qservice.GetQuestionsFormBySetID(LCFRequestModel));

        }


        [MIEAuthorize]
        [HttpPost]
        public ActionResult<string> CreateReferenceLink(int RequestID)
        {

            Guid? guid = _qservice.GetReferenceFormGUUID(RequestID);

            if (guid.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                return BadRequest("Invalid Request Id!!!");
            }
            return Ok(_qservice.CreateReferenceLink(guid));

        }

        [AllowAnonymous]
        [HttpGet("uniqId")]
        public ContentResult GetReferenceCheckFormByGUUID(Guid uniqId)
        {
            string formDataS = string.Empty;

            if (_qservice.IsFormStillValid(uniqId))
            {
                byte[] formData = _qservice.ReferenceCheck(uniqId);
                formDataS = System.Text.Encoding.UTF8.GetString(formData);
            }
            else
            {


                formDataS = StringConstants.INVALID_FORM;
               

            }
            return base.Content(formDataS, "text/html");
        }


        [MIEAuthorize]
        [HttpGet("RequestID")]
        public ContentResult GetReferenceCheckFormByRequestID(int RequestID)
        {

            Guid? uniqId = _qservice.GetReferenceFormGUUID(RequestID);
            byte[] formData = _qservice.ReferenceCheck(uniqId);
            string formDataS = System.Text.Encoding.UTF8.GetString(formData);
            return base.Content(formDataS, "text/html");
        }

        [AllowAnonymous]
        [HttpGet("guuid")]
        public ContentResult NoConsentToReference(Guid guuid)
        {
            LNoConsentModel model = new LNoConsentModel();
            model.status = "no-consent";
            model.formId = guuid;
            //string formDataS = StringConstants.REF_FORM_MAINHEADER + StringConstants.NOCONSENT_RESPONSE;
            string formDataS = _qservice.NoConsentToReference(model);
            return base.Content(formDataS, "text/html");   // leave it to return string for api extension.
        }



        //Questions CRUD
        //--------------

        [MIEAuthorize]
        [HttpGet]
        public List<Question> GetAllQuestions()
        {
            return _qservice.GetAllQuestions();
        }

        [MIEAuthorize]
        [HttpGet("QuestionSetID")]
        public List<Question> GetQuestionsBySetID(int QuestionSetID)
        {
            return _qservice.GetQuestionsBySetID(QuestionSetID);
        }

        [MIEAuthorize]
        [HttpGet("Text")]
        public List<Question> GetQuestionsByText(string Text)
        {
            return _qservice.GetQuestionsByText(Text);
        }

        [MIEAuthorize]
        [HttpPost]
        public Question CreateQuestion([FromForm] Question question)
        {
            Question TQuestion = new Question();
            if (question != null)
            {

                TQuestion.QuestionId = question.QuestionId;
                TQuestion.QuestionSetId = question.QuestionSetId;
                TQuestion.QuestionText = question.QuestionText;
                TQuestion.IsActive = question.IsActive;
                TQuestion.AnswerTypeId = question.AnswerTypeId;
                TQuestion.AnswerGroupId = question.AnswerGroupId;
                TQuestion.LeadingQuestion = question.LeadingQuestion;
                TQuestion.LeadingQuestionAnswer = question.LeadingQuestionAnswer;

                return _qservice.CreateQuestion(TQuestion);

            }
            else
            {
                TQuestion = new Question();
                return TQuestion;
            }
        }

        [MIEAuthorize]
        [HttpPut("{id}")]
        public Question UpdateQuestion(int id, Question question)
        {
            //To fix 
            if (id == question.QuestionId)
            {


                var questionToUpdate = _qservice.GetQuestionById(id);
                if (questionToUpdate != null)
                {
                    return _qservice.UpdateQuestion(question);
                }
                else
                {
                    return question;
                }
            }
            else
            {
                return question;
            }

        }

        [MIEAuthorize]
        [HttpDelete("Id")]
        public bool DeleteQuestion(int id)
        {

            var questionToDelete = _qservice.GetQuestionById(id);

            if (questionToDelete != null)
            {
                return _qservice.DeleteQuestion(id);
            }
            else
            {
                return false;
            }

        }



        #region: Hide For testing only
        //[HttpPost("sendComms")]
        //[ProducesResponseType(typeof(string), 200)] // OK
        //[ProducesResponseType(typeof(string), 400)] // Bad Request
        //public async Task<IActionResult> SendComms(LReminderModel commsData)
        //{
        //    try
        //    {
        //        await _commsService.SendComms(commsData);
        //        return Ok("Reminders sent successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        return BadRequest($"Error sending reminders: {ex.Message}");
        //    }
        //}
        #endregion

        [MIEAuthorize]
        [HttpGet("requestKey")]
        public ContentResult GetAManuallyFillForm(string requestKey)
        {
            string formData = _qservice.GetAManuallyFillForm(requestKey);
            return base.Content(formData, "text/html");
        }


        //-------------------
        //New method
        
        [HttpGet("RequestKey")]
        public bool RenewReference(string RequestKey)
        {

            bool renewStatus = true;

            return renewStatus;


        }

    }
}
