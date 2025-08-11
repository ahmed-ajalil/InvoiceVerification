$(document).ready(function () {
    // Initialize bot configuration display
    showBotConfiguration();

    // Temperature range input change event handler
    $('#temperatureRange').on('input', function () {
        $('#temperatureValue').text($(this).val());
    });

    // MaxTokens range input change event handler
    $('#maxTokensRange').on('input', function () {
        $('#maxTokensValue').text($(this).val());
    });
});

// Logout and Feedback handling
function LogoutandFeedback(type) {
    debugger
    if (type === "logoutaction") {
        window.location.href = actionurl._Feedback + '?type=' + encodeURIComponent('logaction');
    } else if (type === "logoutactionAd") {
        window.location.href = actionurl._Feedback + '?type=' + encodeURIComponent('logoutactionforad');
    }
}

// Fetch and display user admin information
function getUserAdmin() {
    $.ajax({
        url: actionurl._getUserAdmin,
        type: 'GET',
        async: false,
        success: function (response) {
            $('#adminuserid').text(response);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching user admin:', error);
        }
    });
}

// Display loader
function show_loader() {
    $('#loader').css('visibility', 'visible');
}

// Hide loader
function hide_loader() {
    $('#loader').css('visibility', 'hidden');
}

// Dummy page loader function (can be improved or removed)
function PageLoader() {
    show_loader();
    setTimeout(() => {
        hide_loader();
    }, 500);
}

// Show Bot Configuration
function showBotConfiguration() {
    $.ajax({
        url: actionurl._GetBotConfiguration,
        type: 'GET',
        success: function (data) {
            console.log('Bot configuration:', data);

            // Set configuration values in the UI
            $('#openaiApiVersion').text(data.openaiApiVersion || '');
            $('#temperatureValue').text(data.temperature || '0');
            $('#temperatureRange').val(data.temperature || '0');
            $('#maxTokensValue').text(data.maxTokens || '0');
            $('#maxTokensRange').val(data.maxTokens || '0');
            $('#azureEndpoint').val(data.azureEndpoint || '');
            $('#openaiApiKey').val(data.openaiApiKey || '');
            $('#openaiApiType').val(data.openaiApiType || '');
            $('#deploymentName').val(data.deploymentName || '');
        },
        error: function (xhr, textStatus, errorThrown) {
            console.error('Error fetching bot configuration:', errorThrown);
        }
    });
}

// Save or Update Bot Configuration
function saveBotConfiguration() {
    const botConfiguration = {
        openaiApiVersion: $('#openaiApiVersion').text(),
        temperature: parseFloat($('#temperatureRange').val()) || 0,
        maxTokens: parseInt($('#maxTokensRange').val()) || 0,
        azureEndpoint: $('#azureEndpoint').val(),
        openaiApiKey: $('#openaiApiKey').val(),
        openaiApiType: $('#openaiApiType').val(),
        deploymentName: $('#deploymentName').val()
    };

    $.ajax({
        url: actionurl._SaveBotConfiguration,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(botConfiguration),
        success: function (response) {
            console.log('Configuration saved:', response);

            if (response.success) {
                Swal.fire({
                    title: "Success",
                    text: "Bot Configuration Saved Successfully",
                    icon: "success"
                });
            } else {
                Swal.fire({
                    title: "Failed",
                    text: "Something went wrong",
                    icon: "error"
                });
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            console.error('Error saving bot configuration:', errorThrown);
        }
    });
}
 