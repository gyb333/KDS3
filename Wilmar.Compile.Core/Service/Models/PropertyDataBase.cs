using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wilmar.Foundation;
using Wilmar.Model.Core.Definitions.Entities;
using Wilmar.Model.Core.Definitions.Entities.DataTypes;
using Wilmar.Model.Core.Definitions.Entities.Validators;

namespace Wilmar.Compile.Core.Service.Models
{
    public abstract class PropertyDataBase<T> : AnnotationMetadataBase where T : MemberBase
    {
        public PropertyDataBase(MemberBase member, EntityMetadata entity, bool isInherit)
        {
            this.Entity = entity;
            this.Member = member as T;
            this.IsInherit = isInherit;
        }

        public override void Initialize(ProjectMetadata project)
        {
            base.Initialize(project);
            var common = Member.Content as CommonDataType;
            if (common != null && common.BaseType == EDataBaseType.Timestamp
                && Entity.DataContext.DatabaseType == EDatabaseType.MySql)
            {
                PropertyType = new DataTypeMetadata(new DateTimeType(), Project, true);
            }
            else
                PropertyType = new DataTypeMetadata(Member.Content, Project, this.IsRequired);

            if (common != null)
                Validator(common);
        }

        public bool IsForeignKey { get; set; }

        public DataTypeMetadata PropertyType { get; private set; }

        public string Name
        {
            get { return Member.Name; }
        }

        public T Member { get; private set; }

        public EntityMetadata Entity { get; private set; }

        public string GetDefaultValue()
        {
            if (!this.PropertyType.IsEntityType)
            {
                var pro = Member.Content.GetType().GetProperty("DefaultValue");
                if (pro != null)
                {
                    var value = pro.GetValue(Member.Content);
                    if (value != null)
                    {
                        return this.PropertyType.GetValueExpression(value.ToString());
                    }
                }
            }
            return "";
        }
        /// <summary>
        /// 是否为继承实体的属性
        /// </summary>
        public bool IsInherit { get; private set; }

        public abstract bool IsRequired { get; }

        #region Annotations
        public void Key()
        {
            Write("System.ComponentModel.DataAnnotations.Key");
        }

        public void Index(bool unique)
        {
            string name = "System.ComponentModel.DataAnnotations.Schema.Index";
            Write(name, $"IsUnique = {unique.ToString().ToLower()}");
        }

        public void NotMapped()
        {
            Write("System.ComponentModel.DataAnnotations.Schema.NotMapped");
        }
        
        public void Column(string name, int index)
        {
            string key = "System.ComponentModel.DataAnnotations.Schema.Column";
            if (!string.IsNullOrEmpty(name))
                Write(key, $"\"{name}\"", $"Order = {index}");
            else
                Write(key, $"Order = {index}");
        }

        public void DatabaseGenerated(string name)
        {
            Write("System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated", $"System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.{name}");
        }

        public void GeneratedProperty(string generate)
        {
            Write("System.ComponentModel.DataAnnotations.Schema.GeneratedProperty", $"\"{generate}\"");
        }

        public void GeneratedProperty(string add, string update)
        {
            Write("System.ComponentModel.DataAnnotations.Schema.GeneratedProperty", $"\"{add}\"", $"\"{update}\"");
        }

        public void Precision(byte? precision, byte? scale)
        {
            if (precision.HasValue)
            {
                Write("System.ComponentModel.DataAnnotations.Precision", precision.Value.ToString(), scale.HasValue ? scale.Value.ToString() : "0");
            }
        }

        private void Validator(CommonDataType type)
        {
            if (type.BaseType == EDataBaseType.Timestamp)
            {
                if (this.Entity.DataContext.DatabaseType == EDatabaseType.MySql)
                    Write("System.ComponentModel.DataAnnotations.MySqlTimestamp");
                else
                    Write("System.ComponentModel.DataAnnotations.Timestamp");
            }
            else
            {
                WriteValidator(type.Validators.OfType<RequiredValidator>().FirstOrDefault());
                WriteValidator(type.Validators.OfType<CompareValidator>().FirstOrDefault());
                var max = type.GetType().GetProperty("MaxValue");
                var min = type.GetType().GetProperty("MinValue");
                if (max != null && min != null)
                    WriteValidator(type.Validators.OfType<RangeValidator>().FirstOrDefault(), type, max.GetValue(type), min.GetValue(type));
                if (type.BaseType == EDataBaseType.String)
                {
                    var strtype = type as StringType;
                    if (strtype.MaxLength.HasValue)
                        WriteValidator(type.Validators.OfType<MaxLengthValidator>().FirstOrDefault(), strtype.MaxLength.Value);
                    if (strtype.MinLength.HasValue)
                        WriteValidator(type.Validators.OfType<MinLengthValidator>().FirstOrDefault(), strtype.MinLength.Value);

                    foreach (var item in type.Validators)
                    {
                        switch (item.ValidatorType)
                        {
                            case EValidatorType.CreditCard:
                            case EValidatorType.Url:
                            case EValidatorType.EmailAddress:
                            case EValidatorType.Phone:
                                WriteValidator(item);
                                break;
                            case EValidatorType.RegularExpression:
                                WriteValidator((RegularExpressionValidator)item);
                                break;
                        }
                    }

                }
            }
        }
        #endregion

        #region WriteValidator
        private void WriteValidator(RequiredValidator validator)
        {
            if (this.IsRequired)
            {
                validator = validator ?? new RequiredValidator();
                if (!validator.AllowEmptyStrings)
                    WriteValidator((ValidatorBase)validator);
                else
                    WriteValidator(validator, $"({nameof(validator.AllowEmptyStrings)}= {validator.AllowEmptyStrings}");
            }
        }
        private void WriteValidator(MaxLengthValidator validator, int value)
        {
            if (value > 0)
                WriteValidator(validator, value.ToString());
        }
        private void WriteValidator(MinLengthValidator validator, int value)
        {
            if (value > 0)
                WriteValidator(validator, value.ToString());
        }
        private void WriteValidator(RangeValidator validator, CommonDataType type, object max1, object min1)
        {
            if (max1 != null || min1 != null)
            {
                Type datatype = Type.GetType($"System.{type.BaseType}");
                string maxstr = max1 != null ? max1.ToString() : datatype.GetField("MaxValue").GetValue(null).ToString();
                string minstr = min1 != null ? min1.ToString() : datatype.GetField("MinValue").GetValue(null).ToString();
                validator = validator ?? new RangeValidator();
                WriteValidator(validator, $"typeof(System.{type.BaseType})", $"\"{minstr}\"", $"\"{maxstr}\"");
            }
        }
        private void WriteValidator(CompareValidator validator)
        {
            if (validator != null) WriteValidator(validator, $"@\"{validator.OtherProperty}\"");
        }
        private void WriteValidator(RegularExpressionValidator validator)
        {
            if (validator != null) WriteValidator(validator, $"@\"{validator.Pattern}\"");
        }
        private void WriteValidator(ValidatorBase validator, params string[] paras)
        {
            WriteValidator("System.ComponentModel.DataAnnotations.", validator, paras);
        }
        private void WriteValidator(string prefix, ValidatorBase validator, params string[] paras)
        {
            if (validator != null)
            {
                if (string.IsNullOrEmpty(validator.ErrorMessage))
                    Write(prefix + validator.ValidatorType.ToString(), paras);
                else
                    Write(prefix + validator.ValidatorType.ToString(), paras.Union(new string[] { "ErrorMessage = @\"" + validator.ErrorMessage + "\"" }).ToArray());
            }
        }
        #endregion
    }
}
