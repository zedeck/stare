using BackendAPI.Database;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata;

namespace BackendAPI.Constants
{
    public class StringConstants
    {
        public const string REF_FORM_MAINHEADER = "<!doctype html>\r\n<html lang=\"en\">\r\n  <head>\r\n    <meta charset=\"utf-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n    <title>Bootstrap demo</title>\r\n    <link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css\" rel=\"stylesheet\" integrity=\"sha384-T3c6CoIi6uLrA9TneNEoa7RxnatzjcDSCmG1MXxSR1GAsXEV/Dwwykc2MPK8M2HN\" crossorigin=\"anonymous\">\r\n</head>\r\n";
        public const string REF_FORM_HEADER1 = "MIE assists organisations to identity the right talent by conducting factual competency-based employment references and background checks.<br>\r\n\t\tSince past performance is often the best predictor of future performance, the best way to verify a candidate's background and job suitability is to conduct\r\n\t\ta throrough reference check.<br>\r\n\t\tAs a referee, please make an objective assessment of the character and abilities of the candidate.<br>\r\n\t\tAny additional comments which will help MIE to determine suitability of the candidate are appreciated.<br><br>";
        public const string REF_FORM_HEADER2 = "This is a reference request from <span class=\"fw-bold\">Managed Integrity Evaluation</span> with regards to <span class=\"fw-bold\"> " +
                                               "CandidateName</span>. <br> Please take a few moments to fill out the below questionnaire. <br> Please answer each question by evaluating this candidate's skills compared to others in the workplace. <br> " +
                                               "Rating type questions are given on a 1-5 scale, where 1 indicates that, compared to others in the workplace, CandidateName never uses this skill.likewise, a rating of 5 indicates that CandidateName uses these skills at all times. <br> " +
                                               "Your honesty will be much appreciated and note that your response will not be shared with the candidate unless you request it.";
        public const string REF_FORM_DISCLAIMER = "<br>\r\n\t\t<br>\r\n\t\t<p class=\"text-danger\">\r\n\t\tThe information contained in this report is confidential and may be legally privilleged. \r\n\t\tThe contents are solely for the use of client to whom it is addressed and other authorized individuals.\r\n\t\t</p>\r\n\t\t<br>";
        public const string REF_FORM_NOCONSENT = "<p class=\"text-danger\">If this request is not relevant to you or you wish to decline please <a class=\"link-opacity-100\" href=\"NoConsentReturnPage\"  id=\"nonConsent\" title=\"noConsent\">click here</a></p><br>";
        public const string NOCONSENT_RESPONSE = "<body>\r\n<script>\r\n alert(\"Ohh noo...We are sorry for the incovinience.\r\n we will notify our client.\");\r\n        window.location.href = 'https://mettus.co.za//';\r\n\r\n    </script>\r\n</body>\r\n</html>";
        public const string REMINDER_SMSMSG = @"
#LINK
#NEWLINE
Dear #REFNAME, 
#NEWLINE
This is a friendly reminder to complete the reference request for #CANNAME.
#NEWLINE
Please complete it today, to ensure the candidate a bright future. 
#NEWLINE
Kindly click on the link : 
#NEWLINE
Regards
#NEWLINE
The MIE References Team
            ";
        public const string REMINDER_EMAILMSG = @"
Dear #REFNAME, 
<br/>
This is a friendly reminder that you have been requested as a reference for #CANNAME.
<br/>
<br/>
Please complete the MIE reference request by clicking on the link below – This process
<br/>
<br/>
only takes a few minutes to complete and will allow #CANNAME to move forward with their application.
<br/>
<br/>
Kindly click on the link : <a href=""#LINK"">Reference questions</a>
<br/>
<br/>
Regards
<br/>
The MIE References Team
           ";
        public const string NEWREFERENCE_SMSMSG = "REFERENCE_LINK\r\nHello REFEREE_NAME,\r\n" +
                                                  "You've been listed as a referee for REFERENCE_NAME.\r\n"+
                                                  "Please take a few minutes, to feedback via this link:\r\n";

        public const string NEWREFERENCE_EMAILMSG =
@"Dear REFEREE_NAME,
<br/>
<br/>
This email is to inform you that REFERENCE_NAME has listed you as a reference on their behalf for a potential job opportunity.
<br/>
<br/>
Please click on the link below to fill out the reference request.
<br/>
<br/>
This process will only take a few minutes of your time to complete. Your urgent attention to this request is highly appreciated by MIE REFERENCE_NAME.
<br/>
<br/>
To fill out the reference, please click the link below, or copy and paste the link into your browser:
<br/>
<br/>
Link:  : <a href=""REFERENCE_LINK"">Reference questions</a>
<br/>
<br/>
Thank you.";

        public const string INVALID_FORM = "<head>\r\n    <link href=\"~/lib/bootstrap-5.3.2-dist/css/bootstrap.css/bootstrap.min.css\">\r\n    <title>Mie Invalid Form!</title>\r\n\r\n</head>\r\n\r\n<body>\r\n\r\n    <!-- Button trigger modal -->\r\n    <!-- <button type=\"button\" class=\"btn btn-primary\" data-bs-toggle=\"modal\" data-bs-target=\"#brokenLinkModal\">\r\n        Modal\r\n    </button> -->\r\n\r\n  <!-- Modal -->\r\n    <div class=\"modal fade\" id=\"brokenLinkModal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"brokenLinkModal\"\r\n        aria-hidden=\"false\">\r\n        <div class=\"modal-dialog\" role=\"document\">\r\n            <div class=\"modal-content\">\r\n                <div class=\"modal-header\">\r\n                    <h5 class=\"modal-title\" id=\"brokenLinkModal\">Broken Link!!!</h5>\r\n                </div>\r\n                <div class=\"modal-body\">\r\n                    <p>We are sorry, the link that you are accessing is now expired, Should you wish to continue please\r\n                        click the 'Request' button below to create a new link or 'Close' to exit.</p>\r\n                </div>\r\n                <div class=\"modal-footer\">\r\n                    <button type=\"button\" class=\"btn btn-danger\" data-bs-dismiss=\"modal\">Close</button>\r\n                    <button type=\"button\" class=\"btn btn-success\" id=\" requestNewLink\" title=\"requestNewLink\"\r\n                        data-bs-dismiss=\"modal\"\r\n                        onclick=\"requestNewLink('2EB493CE-C1B2-41C6-ADD7-7659EEA88E2A')\">Request</button>\r\n                </div>\r\n\r\n\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n\r\n    <script>\r\n        function requestNewLink(param) {\r\n\r\n            //alert(param);\r\n            $.ajax({\r\n                type: \"GET\",\r\n                dataType: \"json\",\r\n                url: \"https://localhost:7060/api/Questionnaire/GetReferenceCheckFormByGUUID/uniqId?uniqId=\" + param,\r\n                success: function (data) {\r\n                    alert(data);\r\n                    //brokenLinkModal.hide();\r\n                    //var tag_id = document.getElementById('someInfo');\r\n                    //tag_id.innerHTML = 'Hello';\r\n                    //alert(\"New link requested successfully, you will receive email and sms shortly.\")\r\n\r\n                },\r\n                error: function (error) {\r\n\r\n                    // jsonValue = jQuery.parseJSON(error.responseText);\r\n                    //alert(\"error\" + error.responseText);\r\n\r\n                }\r\n            });\r\n\r\n        };\r\n\r\n\r\n        \r\n\r\n    </script>\r\n\r\n    <script src=\"~/lib/bootstrap-5.3.2/js/bootstrap.js/bootstrap.bundle.min.js\"\r\n></script>\r\n\r\n    <script src=\"~/lib/jquery/jquery.js/jquery.min.js\"\r\n></script>\r\n\r\n</body>\r\n\r\n";
        //"<div class=\"modal fade\" id=\"brokenLinkModal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"brokenLinkModal\"\r\n        aria-hidden=\"false\">\r\n        <div class=\"modal-dialog\" role=\"document\">\r\n            <div class=\"modal-content\">\r\n                <div class=\"modal-header\">\r\n                    <h5 class=\"modal-title\" id=\"brokenLinkModal\">Broken Link!!!</h5>\r\n                </div>\r\n                <div class=\"modal-body\">\r\n                    <p>We are sorry, the link that you are accessing is now expired, Should you wish to continue please\r\n                        click the 'Request' button below to create a new link or 'Close' to exit.</p>\r\n                </div>\r\n                <div class=\"modal-footer\">\r\n                    <button type=\"button\" class=\"btn btn-danger\" data-bs-dismiss=\"modal\">Close</button>\r\n                    <button type=\"button\" class=\"btn btn-success\" id=\" requestNewLink\" title=\"requestNewLink\"\r\n                        data-bs-dismiss=\"modal\"\r\n                        onclick=\"requestNewLink('2EB493CE-C1B2-41C6-ADD7-7659EEA88E2A')\">Request</button>\r\n                </div>\r\n\r\n\r\n            </div>\r\n        </div>\r\n    </div>";

        //"<!DOCTYPE html>\r\n<html>\r\n\r\n<head>\r\n    <link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css\" rel=\"stylesheet\"\r\n        integrity=\"sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC\" crossorigin=\"anonymous\">\r\n    <title>Mie Invalid Form!</title>\r\n\r\n</head>\r\n\r\n<body>\r\n\r\n    <!-- Button trigger modal -->\r\n    <!-- <button type=\"button\" class=\"btn btn-primary\" data-bs-toggle=\"modal\" data-bs-target=\"#brokenLinkModal\">\r\n        Modal\r\n    </button> -->\r\n\r\n    <!-- <a type=\"link\" data-bs-toggle=\"modal\" data-bs-target=\"#brokenLinkModal\" href=\"\">\r\n        ReferenceLink\r\n    </a> -->\r\n\r\n\r\n    <!-- Modal -->\r\n    <div class=\"modal fade\" id=\"brokenLinkModal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"brokenLinkModal\"\r\n        aria-hidden=\"true\">\r\n        <div class=\"modal-dialog\" role=\"document\">\r\n            <div class=\"modal-content\">\r\n                <div class=\"modal-header\">\r\n                    <h5 class=\"modal-title\" id=\"brokenLinkModal\">Broken Link!!!</h5>\r\n                </div>\r\n                <div class=\"modal-body\">\r\n                    <p>We are sorry, the link that you are accessing is now expired, Should you wish to continue please\r\n                        click the 'Request' button below to create a new link or 'Close' to exit.</p>\r\n                </div>\r\n                <div class=\"modal-footer\">\r\n                    <button type=\"button\" class=\"btn btn-danger\" data-bs-dismiss=\"modal\">Close</button>\r\n                    <button type=\"button\" class=\"btn btn-success\" id=\" requestNewLink\" title=\"requestNewLink\"\r\n                        data-bs-dismiss=\"modal\"\r\n                        onclick=\"requestNewLink('2EB493CE-C1B2-41C6-ADD7-7659EEA88E2A')\">Request</button>\r\n                </div>\r\n\r\n\r\n            </div>\r\n        </div>\r\n    </div>\r\n\r\n\r\n\r\n\r\n\r\n    <script>\r\n        function requestNewLink(param) {\r\n\r\n            //alert(param);\r\n            $.ajax({\r\n                type: \"GET\",\r\n                dataType: \"json\",\r\n                url: \"https://localhost:7060/api/Questionnaire/GetReferenceCheckFormByGUUID/uniqId?uniqId=\" + param,\r\n                success: function (data) {\r\n                    alert(data);\r\n                    //brokenLinkModal.hide();\r\n                    //var tag_id = document.getElementById('someInfo');\r\n                    //tag_id.innerHTML = 'Hello';\r\n                    //alert(\"New link requested successfully, you will receive email and sms shortly.\")\r\n\r\n                },\r\n                error: function (error) {\r\n\r\n                    // jsonValue = jQuery.parseJSON(error.responseText);\r\n                    //alert(\"error\" + error.responseText);\r\n\r\n                }\r\n            });\r\n\r\n        };\r\n\r\n    </script>\r\n\r\n    <script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.bundle.min.js\"\r\n        integrity=\"sha384-MrcW6ZMFYlzcLA8Nl+NtUVF0sA7MsXsP1UyJoMp4YLEuNSfAP+JcXn/tWtIaxVXM\"\r\n        crossorigin=\"anonymous\"></script>\r\n\r\n    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js\"\r\n        integrity=\"sha512-v2CJ7UaYy4JwqLDIrZUI/4hqeoQieOmAZNXBeQyjo21dadnwR+8ZaIJVT8EE2iyI61OV8e6M8PP2/4hpQINQ/g==\"\r\n        crossorigin=\"anonymous\" referrerpolicy=\"no-referrer\"></script>\r\n\r\n</body>\r\n\r\n</html>";

        public const string INTERNAL_EMAIL_NOTICE = @"
Dear #SIG_NAME, 
<br/>
<br/>
Please be advised that the reference check for following request has been completed and is ready for QA and approval.
<br/>
<br/>
<table border = 1>
    <tr>
        <td><b>Candidate name</b></td>
        <td>#CANNAME </td>
    </tr>
    <tr>
        <td><b>PCV Number</b></td>
        <td><a href=""#REFERENCE_LINK"">#REMOTE_KEY</a> </td>
    </tr>
</table>
<br/>
<br/>
Please click on the PCV number to direct you to the questionnaire responses submitted by the referee.

<br/>
<br/>
Regards
<br/>
The MIE References Team
           ";

    }
}
