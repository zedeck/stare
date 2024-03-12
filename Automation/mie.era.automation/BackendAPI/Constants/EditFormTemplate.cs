namespace BackendAPI.Constants
{
    public class EditFormTemplate
    {
        public const string DOCUMENT_MAIN = "<!DOCTYPE html>\r\n" +
                                            "<html>\r\n" +
                                                "\r\n{HEAD_SECTION}\r\n" +
                                                "\r\n{BODY_SECTION}\r\n" +
                                            "</html>";

        public const string HEAD_SECTION = "<head>{STYLING_SECTION}</head>";
        public const string BODY_SECTION = "<body>{CONTENT_SECTION}{SCRIPTS_SECTION}</body>";
        public const string CONTENT_SECTION = "<h4>Editing answers for {REQUESTKEY}</h4>";
        public const string SCRIPTS_SECTION =  "<script>\r\n\r\n    $('#submitForm').click(function () {\r\n\r\n      const forms = document.querySelectorAll('.needs-validation')\r\n      Array.from(forms).forEach(form => {\r\n        form.addEventListener('submit', event => {\r\n          if (!form.checkValidity()) {\r\n            event.preventDefault()\r\n            event.stopPropagation()\r\n\r\n            $(form).find(\".form-control:invalid\").first().focus();\r\n\r\n          }\r\n\r\n          form.classList.add('was-validated')\r\n        }, false)\r\n\r\n      })\r\n\r\n      if ($('form')[0].checkValidity()) {\r\n\r\n        swal({\r\n          title: \"Thank You!\",\r\n          text: \"Your approval is submitted!!!\",\r\n          type: \"success\"\r\n        }).then(function () {\r\n   window.location.href =\"http://qaweb01.miegalaxy.com/Internal/apps/era-mvc-int/api/Referees/Index\"; \r\n        });\r\n\r\n      }\r\n\r\n    });\r\n  </script>";
        public const string STYLING_SECTION = "<script src=\"https://code.jquery.com/jquery-3.7.1.js\" integrity=\"sha256-eKhayi8LEQwp4NKxN+CfCh+3qOVUtJn3QNZ0TciWLP4=\"\r\n    crossorigin=\"anonymous\"></script>\r\n" +
                                              "<script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js\" integrity=\"sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz\" crossorigin=\"anonymous\"></script>" +
                                              "<link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css\" rel=\"stylesheet\" integrity=\"sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH\" crossorigin=\"anonymous\">" +
                                              "<script src=\"https://cdn.jsdelivr.net/npm/sweetalert2@7.12.15/dist/sweetalert2.all.min.js\"></script>" +
                                              "<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/sweetalert2@7.12.15/dist/sweetalert2.min.css'>\r\n  </link>" +
                                              "<script\r\n    src=\"https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/0.9.13/js/bootstrap-multiselect.js\"></script>" +
                                              "<link rel=\"stylesheet\"\r\n    href=\"https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/0.9.13/css/bootstrap-multiselect.css\">" +
                                              "<style>\r\n   .fontStyle {\r\n      color: #ffaa0d;\r\n      font-size: 20px;\r\n    }  .rating {\r\n      display: flex;\r\n      flex-direction: row-reverse;\r\n      justify-content: start;\r\n      gap: 25px;\r\n\r\n    }\r\n\r\n    .rating>input {\r\n      display: none\r\n    }\r\n\r\n    .rating>label {\r\n      position: relative;\r\n      width: 1em;\r\n      font-size: 30px;\r\n      font-weight: 300;\r\n      color: #FFD600;\r\n      cursor: pointer\r\n    }\r\n\r\n    .rating>label::before {\r\n      content: \"\\2605\";\r\n      position: absolute;\r\n      opacity: 0\r\n    }\r\n\r\n    .rating>label:hover:before,\r\n    .rating>label:hover~label:before {\r\n      opacity: 1 !important\r\n    }\r\n\r\n    .rating>input:checked~label:before {\r\n      opacity: 1\r\n    }\r\n\r\n    .rating:hover>input:checked~label:before {\r\n      opacity: 0.4\r\n    }\r\n  </style>";
                                              



    }
}
