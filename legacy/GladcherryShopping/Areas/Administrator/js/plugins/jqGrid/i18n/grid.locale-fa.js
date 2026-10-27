;(function($){
/**
 * jqGrid English Translation
 * Tony Tomov tony@trirand.com
 * http://trirand.com/blog/ 
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/
$.jgrid = $.jgrid || {};
$.extend($.jgrid,{
	defaults : {
		recordtext: "مشاهده {0} - {1} از {2}",
		emptyrecords: "رکوردی یافت نشد.",
		loadtext: "بارگذاری...",
		pgtext : "صفحه {0} از {1}"
	},
	search : {
		caption: "جستجو...",
		Find: "یافتن",
		Reset: "دوباره",
		odata: [{ oper:'eq', text:'equal'},{ oper:'ne', text:'not equal'},{ oper:'lt', text:'less'},{ oper:'le', text:'less or equal'},{ oper:'gt', text:'greater'},{ oper:'ge', text:'greater or equal'},{ oper:'bw', text:'begins with'},{ oper:'bn', text:'does not begin with'},{ oper:'in', text:'is in'},{ oper:'ni', text:'is not in'},{ oper:'ew', text:'ends with'},{ oper:'en', text:'does not end with'},{ oper:'cn', text:'contains'},{ oper:'nc', text:'does not contain'},{ oper:'nu', text:'is null'},{ oper:'nn', text:'is not null'}],
		groupOps: [{ op: "AND", text: "all" },{ op: "OR",  text: "any" }],
		operandTitle : "Click to select search operation.",
		resetTitle : "Reset Search Value"
	},
	edit : {
		addCaption: "افزودن رکورد",
		editCaption: "ویرایش رکورد",
		bSubmit: "ارسال",
		bCancel: "لغو",
		bClose: "خروج",
		saveData: "اطلاعات تغییر کرد ! ذخیره گردد ؟",
		bYes : "بله",
		bNo : "خیر",
		bExit : "لغو",
		msg: {
			required:"فیلد الزامی است.",
			number:"لطفا عدد صحیح وارد نمایید.",
			minValue:"مقدار وارد شده می بایست بزرگتر یا مساوی مقدار ربرو باش ",
			maxValue:"مقدار وارد شده می بایست کوچکتر یا مساوی مقدار روبرو باشد ",
			email: "ایمیل معتبر نیست",
			integer: "لطفا یک مقدار عددی صحیح وارد نمایید.",
			date: "لطفا یک مقدار تاریخی صحیح وارد نمایید.",
			url: "یک آدرس صحیح نیست. در ابتدای آدرس وجود http:// و یا https:// الزامی است.",
			nodefined : " تعریف نشده است!",
			novalue : " مقدار بازگشتی الزامی است!",
			customarray : "متد دلخواه می بایست آرایه برگرداند!",
			customfcheck : "متد دلخواه می بایست در مورد چک کردن حاظر باشد!"
			
		}
	},
	view : {
		caption: "مشاهده رکورد",
		bClose: "خروج"
	},
	del : {
		caption: "حذف",
		msg: "رکورد(های) انتخابی حذف گردد ؟",
		bSubmit: "حذف",
		bCancel: "لغو"
	},
	nav : {
		edittext: "",
		edittitle: "ویرایش سطرهای انتخاب شده",
		addtext:"",
		addtitle: "افزودن سطر جدید",
		deltext: "",
		deltitle: "حذف سطرهای انتخاب شده",
		searchtext: "",
		searchtitle: "یافتن رکوردها",
		refreshtext: "",
		refreshtitle: "بارگذاری جدول",
		alertcap: "اخطار",
		alerttext: "لطفا سطری لنتخاب کنید",
		viewtext: "",
		viewtitle: "مشاهده سطرهای انتخابی"
	},
	col : {
		caption: "انتخاب ستون ها",
		bSubmit: "تایید",
		bCancel: "لغو"
	},
	errors : {
		errcap : "خطا",
		nourl : "هیچ آدرس تعیین نشده است.",
		norecords: "هیچ رکوردی برای پردازش وجود ندارد.",
		model : "Length of colNames <> colModel!"
	},
	formatter : {
		integer : {thousandsSeparator: ",", defaultValue: '0'},
		number : {decimalSeparator:".", thousandsSeparator: ",", decimalPlaces: 2, defaultValue: '0.00'},
		currency : {decimalSeparator:".", thousandsSeparator: ",", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0.00'},
		date : {
			dayNames:   [
				"ی", "د", "س", "چ", "پ", "ج", "ش",
				"یکشنبه", "دئشنبه", "سه شنبه", "چهارشنبه", "پنج شنبه", "جمعه", "شنبه"
			],
			monthNames: [
				"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
				"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
			],
			AmPm : ["am","pm","AM","PM"],
			S: function (j) {return j < 11 || j > 13 ? ['st', 'nd', 'rd', 'th'][Math.min((j - 1) % 10, 3)] : 'th';},
			srcformat: 'Y-m-d',
			newformat: 'n/j/Y',
			parseRe : /[#%\\\/:_;.,\t\s-]/,
			masks : {
				// see http://php.net/manual/en/function.date.php for PHP format used in jqGrid
				// and see http://docs.jquery.com/UI/Datepicker/formatDate
				// and https://github.com/jquery/globalize#dates for alternative formats used frequently
				// one can find on https://github.com/jquery/globalize/tree/master/lib/cultures many
				// information about date, time, numbers and currency formats used in different countries
				// one should just convert the information in PHP format
				ISO8601Long:"Y-m-d H:i:s",
				ISO8601Short:"Y-m-d",
				// short date:
				//    n - Numeric representation of a month, without leading zeros
				//    j - Day of the month without leading zeros
				//    Y - A full numeric representation of a year, 4 digits
				// example: 3/1/2012 which means 1 March 2012
				ShortDate: "n/j/Y", // in jQuery UI Datepicker: "M/d/yyyy"
				// long date:
				//    l - A full textual representation of the day of the week
				//    F - A full textual representation of a month
				//    d - Day of the month, 2 digits with leading zeros
				//    Y - A full numeric representation of a year, 4 digits
				LongDate: "l, F d, Y", // in jQuery UI Datepicker: "dddd, MMMM dd, yyyy"
				// long date with long time:
				//    l - A full textual representation of the day of the week
				//    F - A full textual representation of a month
				//    d - Day of the month, 2 digits with leading zeros
				//    Y - A full numeric representation of a year, 4 digits
				//    g - 12-hour format of an hour without leading zeros
				//    i - Minutes with leading zeros
				//    s - Seconds, with leading zeros
				//    A - Uppercase Ante meridiem and Post meridiem (AM or PM)
				FullDateTime: "l, F d, Y g:i:s A", // in jQuery UI Datepicker: "dddd, MMMM dd, yyyy h:mm:ss tt"
				// month day:
				//    F - A full textual representation of a month
				//    d - Day of the month, 2 digits with leading zeros
				MonthDay: "F d", // in jQuery UI Datepicker: "MMMM dd"
				// short time (without seconds)
				//    g - 12-hour format of an hour without leading zeros
				//    i - Minutes with leading zeros
				//    A - Uppercase Ante meridiem and Post meridiem (AM or PM)
				ShortTime: "g:i A", // in jQuery UI Datepicker: "h:mm tt"
				// long time (with seconds)
				//    g - 12-hour format of an hour without leading zeros
				//    i - Minutes with leading zeros
				//    s - Seconds, with leading zeros
				//    A - Uppercase Ante meridiem and Post meridiem (AM or PM)
				LongTime: "g:i:s A", // in jQuery UI Datepicker: "h:mm:ss tt"
				SortableDateTime: "Y-m-d\\TH:i:s",
				UniversalSortableDateTime: "Y-m-d H:i:sO",
				// month with year
				//    Y - A full numeric representation of a year, 4 digits
				//    F - A full textual representation of a month
				YearMonth: "F, Y" // in jQuery UI Datepicker: "MMMM, yyyy"
			},
			reformatAfterEdit : false
		},
		baseLinkUrl: '',
		showAction: '',
		target: '',
		checkbox : {disabled:true},
		idName : 'id'
	}
});
})(jQuery);
