$(function () {
    $(document).on("change", "input[name^='question']", function () {
        var allGood = true;
        var lastInputField = parseInt($("input[name^='question']").last().attr("id").match(/\d+/)[0]);

        $("input[name^='question']").each(function () {
            if ($(this).val() === "") {
                allGood = false;
                return false;
            }
        });

        if (allGood) {
            var newInputField = $("<div><input type='text' name='question" + (lastInputField + 1) + "' id='question" + (lastInputField + 1) + "' placeholder='question " + (lastInputField + 1) + "'></div>");
            $("form").append(newInputField);
        }
    });
});
