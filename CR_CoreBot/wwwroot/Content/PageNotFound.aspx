<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Page Not Found</title>
    <link href="..Content/bootstrap.min.css" rel="stylesheet" />    
    <script src="../Scripts/jquery-1.10.2.min.js"></script>
    <script src="../Scripts/bootstrap.min.js"></script>
    <script src="../CustomJs/SupportAndFeedbackJs.js"></script>
    <style>
        body {
            background: #eaeaea;
        }

        .wrap {
            margin: 0 auto;
            width: 1000px;
        }

        .content-msg {
            text-align: center;
            margin-top: 200px;
        }

            .content-msg p {
                color: #272727;
                font-size: 40px;
                margin-top: 1px;
                font-family: "Segoe", "Segoe UI", "DejaVu Sans", "Trebuchet MS", Verdana, sans-serif;
            }

            .content-msg h2 {
                font-size: 10em;
                color: #0f6d84;
                margin: 0px 0px 30px;
                font-weight: bold;
                text-shadow: 0px 5px 1px #514F4F;
                position: relative;
            }

                .content-msg h2::before {
                    content: '';
                    width: 130px;
                    height: 127px;
                    background: url(../images/1.png) no-repeat 0px 0px;
                    display: block;
                    position: absolute;
                    left: 4%;
                    top: 38%;
                }
    </style>
</head>
<body>
    <div class="wrap">
        <div class="content-msg">
            <div class="row">
                <div class="col-md-12">
                    <div class="error-template">
                        <p>
                            Unauthorised Access!!
                        </p>
                        <h2>
                            <%--404--%>
                        </h2>
                        <div class="bg-danger" style="display:inline-block; padding:5px 10px; margin:10px 0px;">
                            Sorry, an error has occured, Requested page not found!
                        </div>
                        <div class="error-actions">
                            <a href="/Dashboard/Index" class="btn btn-primary btn-lg">
                                <span class="glyphicon glyphicon-home"></span>
                                Take Me Home
                            </a>
                            <a id="lbtnSupport" class="btn btn-default btn-lg" data-toggle="modal" data-target="#myModalSupport" data-backdrop="static" data-keyboard="false">
                                <span class="glyphicon glyphicon-envelope"></span> Contact Support
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!--*****************************************Feedback Model*************************************************-->
    <div class="modal fade" id="myModalSupport" role="dialog">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="loadingImg" id="divLoading" style="visibility:hidden;">
                    <img src="~/img/Authochecking-loader.gif" />
                </div>
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h3>CloudPilot Support</h3>
                </div>
                <div class="modal-body">
                    <div class="container-fluid" style="width: 100%; margin-left: 0px;">
                        <div class="row">
                            <div class="col-sm-12" id="divcontain">
                                <div class="form-group">
                                    <label>Subject <sup style="color: red;">*</sup></label>
                                    <input type="text" id="txtSubject" class="form-control" placeholder="Subject" maxlength="100" />
                                    <span id="spanSubject" style="color: red; font-weight: bold; display:none;">Enter Subject</span>
                                    <span id="spanSubjectVal" style="color: red; font-weight: bold; display:none;">Enter Subject Properly</span>
                                </div>
                                <div class="form-group">
                                    <label>Select Problem Type <sup style="color: red;">*</sup></label>
                                    <select id="ddlQuerytype" class="form-control" onchange="showTextBox();">
                                        <option value="[Select]">[Select]</option>
                                        <option value="Login Issue">Login Issue</option>
                                        <option value="Cannot manage my profile">Cannot manage my profile</option>
                                        <option value="Cannot manage my subscriptions">Cannot manage my subscriptions</option>
                                        <option value="General tool navigation">General tool navigation</option>
                                        <option value="How to manage my profile">How to manage my profile</option>
                                        <option value="How to manage my subscriptions">How to manage my subscriptions</option>
                                        <option value="How to manage users">How to manage users</option>
                                        <option value="License-based billing">License-based billing</option>
                                        <option value="Usage-based billing">Usage-based billing</option>
                                        <option value="General Inquiry">General Inquiry</option>
                                        <option value="Other">Other</option>
                                    </select>
                                    <span id="spanProblemType" style="color: red; font-weight: bold; display:none;">Select Problem Type</span>
                                </div>

                                <div class="form-group" id="divOther" style="display:none">
                                    <input type="text" id="txtOther" class="form-control" placeholder="Other Problems" maxlength="100" />
                                    <span id="spanOther" style="color: red; font-weight: bold; display:none;">Enter Other Problems</span>

                                </div>

                                <div class="form-group">
                                    <label>Message [Message length 250 characters] <sup style="color: red;">*</sup></label>
                                    <textarea id="txtQuery" class="form-control" placeholder="Message" rows="3" maxlength="250" style="resize:none;"></textarea>
                                    <span id="spanMessage" style="color: red; font-weight: bold; display:none;">Enter Message</span>
                                    <span id="spanMessageVal" style="color: red; font-weight: bold; display:none;">Enter Message Properly</span>
                                </div>
                                <div class="form-group">
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <br />
                                        <label id="lblmsg" class="msgfont"></label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" onclick="btnCloseClick()" class="btn btn-default" data-dismiss="modal">Close</button>
                    <button type="button" id="btnSubmit" class="btn btn-info" onclick="btnSupportClick()">Submit</button>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
