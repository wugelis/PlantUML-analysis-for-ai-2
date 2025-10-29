// 車輛租用系統 JavaScript 功能

$(document).ready(function() {
    // 初始化所有功能
    initCarTypeSelector();
    initDateValidation();
    initFeeCalculation();
    initFormValidation();
});

// 車輛類型選擇器
function initCarTypeSelector() {
    $('.car-type-option').on('click', function() {
        $('.car-type-option').removeClass('selected');
        $(this).addClass('selected');
        
        const carType = $(this).data('car-type');
        $('#CarType').val(carType);
        
        // 載入該類型的可用車輛
        loadAvailableCars();
        
        // 重新計算租金
        calculateFee();
    });
}

// 日期驗證
function initDateValidation() {
    const today = new Date().toISOString().split('T')[0];
    
    // 設定最小日期為今天
    $('#StartDate').attr('min', today);
    
    $('#StartDate').on('change', function() {
        const startDate = $(this).val();
        if (startDate) {
            // 結束日期最小為開始日期的下一天
            const minEndDate = new Date(startDate);
            minEndDate.setDate(minEndDate.getDate() + 1);
            $('#EndDate').attr('min', minEndDate.toISOString().split('T')[0]);
            
            // 如果結束日期早於開始日期，重設結束日期
            const endDate = $('#EndDate').val();
            if (endDate && new Date(endDate) <= new Date(startDate)) {
                $('#EndDate').val(minEndDate.toISOString().split('T')[0]);
            }
        }
        
        loadAvailableCars();
        calculateFee();
    });
    
    $('#EndDate').on('change', function() {
        loadAvailableCars();
        calculateFee();
    });
}

// 載入可用車輛
function loadAvailableCars() {
    const carType = $('#CarType').val();
    const startDate = $('#StartDate').val();
    const endDate = $('#EndDate').val();
    
    if (!carType || !startDate || !endDate) return;
    
    const $carSelect = $('#CarId');
    $carSelect.html('<option value="">載入中...</option>').prop('disabled', true);
    
    $.post('/Rental/LoadCarsByType', {
        carType: carType,
        startDate: startDate,
        endDate: endDate
    })
    .done(function(data) {
        $carSelect.html('<option value="">請選擇車輛</option>');
        
        if (data && data.length > 0) {
            $.each(data, function(index, car) {
                $carSelect.append(
                    $('<option>', {
                        value: car.carId,
                        text: car.displayText
                    })
                );
            });
        } else {
            $carSelect.append('<option value="">此時段無可用車輛</option>');
        }
    })
    .fail(function() {
        $carSelect.html('<option value="">載入失敗，請重試</option>');
    })
    .always(function() {
        $carSelect.prop('disabled', false);
    });
}

// 租金計算
function initFeeCalculation() {
    $('#CarId').on('change', calculateFee);
}

function calculateFee() {
    const carType = $('#CarType').val();
    const startDate = $('#StartDate').val();
    const endDate = $('#EndDate').val();
    
    if (!carType || !startDate || !endDate) {
        $('#fee-display').hide();
        return;
    }
    
    $('#fee-display .loading').show();
    
    $.post('/Rental/CalculateFee', {
        carType: carType,
        startDate: startDate,
        endDate: endDate
    })
    .done(function(response) {
        if (response.success) {
            $('#rental-days').text(response.days);
            $('#rental-fee').text('NT$' + response.fee.toLocaleString());
            $('#fee-display').show().addClass('fade-in');
        } else {
            showError('計算租金時發生錯誤：' + (response.error || '未知錯誤'));
        }
    })
    .fail(function() {
        showError('計算租金時發生網路錯誤');
    })
    .always(function() {
        $('#fee-display .loading').hide();
    });
}

// 表單驗證
function initFormValidation() {
    $('form').on('submit', function(e) {
        const form = this;
        
        // 清除之前的錯誤狀態
        $(form).find('.is-invalid').removeClass('is-invalid');
        
        // 檢查必填欄位
        let isValid = true;
        $(form).find('[required]').each(function() {
            if (!$(this).val().trim()) {
                $(this).addClass('is-invalid');
                isValid = false;
            }
        });
        
        // 檢查日期邏輯
        const startDate = $('#StartDate').val();
        const endDate = $('#EndDate').val();
        if (startDate && endDate && new Date(endDate) <= new Date(startDate)) {
            $('#EndDate').addClass('is-invalid');
            showError('結束日期必須晚於開始日期');
            isValid = false;
        }
        
        if (!isValid) {
            e.preventDefault();
            return false;
        }
        
        // 顯示載入狀態
        const $submitBtn = $(form).find('button[type="submit"]');
        const originalText = $submitBtn.text();
        $submitBtn.prop('disabled', true)
                  .html('<span class="loading"></span> 處理中...');
        
        // 如果表單驗證失敗，恢復按鈕狀態
        setTimeout(function() {
            if (!form.checkValidity()) {
                $submitBtn.prop('disabled', false).text(originalText);
            }
        }, 100);
    });
}

// 工具函數
function showError(message) {
    const alertHtml = `
        <div class="alert alert-danger alert-dismissible fade show" role="alert">
            <i class="fas fa-exclamation-circle"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    // 移除現有的錯誤訊息
    $('.alert-danger').remove();
    
    // 在頁面頂部顯示錯誤訊息
    $('main').prepend('<div class="container mt-3">' + alertHtml + '</div>');
    
    // 滾動到頂部
    $('html, body').animate({ scrollTop: 0 }, 300);
}

function showSuccess(message) {
    const alertHtml = `
        <div class="alert alert-success alert-dismissible fade show" role="alert">
            <i class="fas fa-check-circle"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    // 移除現有的成功訊息
    $('.alert-success').remove();
    
    // 在頁面頂部顯示成功訊息
    $('main').prepend('<div class="container mt-3">' + alertHtml + '</div>');
    
    // 滾動到頂部
    $('html, body').animate({ scrollTop: 0 }, 300);
}

// 格式化數字為貨幣格式
function formatCurrency(amount) {
    return 'NT$' + amount.toLocaleString();
}

// 計算兩個日期之間的天數
function daysBetween(date1, date2) {
    const oneDay = 24 * 60 * 60 * 1000;
    const firstDate = new Date(date1);
    const secondDate = new Date(date2);
    
    return Math.round(Math.abs((firstDate - secondDate) / oneDay));
}